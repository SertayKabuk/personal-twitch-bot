using Microsoft.Extensions.Logging;
using StreamlabsLib.Events;
using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace StreamlabsLib
{
    internal class StreamlabsSocketClient
    {
        private ClientWebSocket Client { get; set; }
        private readonly string socketUrl = "wss://sockets.streamlabs.com/socket.io/?EIO=3&transport=websocket&token=";
        private bool isConnected;
        private Task[] _networkTasks;
        private CancellationTokenSource _tokenSource = new();
        private readonly System.Timers.Timer pingTimer;
        private readonly System.Timers.Timer pongTimer;
        private readonly ILogger logger;

        public event EventHandler<OnFatalErrorEventArgs> OnFatality;
        public event EventHandler<OnConnectedEventArgs> OnConnected;
        public event EventHandler<OnDisconnectedEventArgs> OnDisconnected;
        public event EventHandler<OnMessageEventArgs> OnMessage;
        public event EventHandler<OnReconnectedEventArgs> OnReconnected;

        private static readonly byte[] PING_STRING = Encoding.UTF8.GetBytes("2");
        private static readonly byte[] PONG_STRING = Encoding.UTF8.GetBytes("3");
        private bool pongReceived = false;

        private bool needReconnect = false;

        private int pingInterval;
        private int pingTimeout;
        private string sessionId;
        private string socketToken;

        public StreamlabsSocketClient(ILogger logger)
        {
            pingTimer = new System.Timers.Timer
            {
                AutoReset = true
            };
            pingTimer.Elapsed += PingElapsed;

            pongTimer = new System.Timers.Timer
            {
                AutoReset = false
            };
            pongTimer.Elapsed += PongElapsed;
            this.logger = logger;
        }

        private void SetSocketToken(string socketToken)
        {
            this.socketToken = socketToken;
        }

        private void PongElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (!pongReceived)
            {
                ReConnect();
                pingTimer.Stop();
            }
        }

        private void PingElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            Client.SendAsync(new ArraySegment<byte>(PING_STRING), WebSocketMessageType.Text, true, CancellationToken.None);

            pongReceived = false;
            pongTimer.Start();
        }

        public void Connect(string socketToken)
        {
            logger.LogInformation("Streamlabs socket connect started");

            this.socketToken = socketToken;

            needReconnect = false;

            Client = new ClientWebSocket();

            Client.ConnectAsync(new Uri(socketUrl + socketToken), _tokenSource.Token).Wait(10000, _tokenSource.Token);
            isConnected = true;

            _networkTasks = new[]
            {
                StartListenerTask(socketToken),
                ListenReconnectNeeded()
            }.ToArray();

            if (!_networkTasks.Any(c => c.IsFaulted))
            {
                OnConnected?.Invoke(this, new OnConnectedEventArgs());
                return;
            }

            Close();
        }

        private Task ListenReconnectNeeded()
        {
            return Task.Run(async () =>
            {
                logger.LogInformation("Streamlabs ListenReconnectNeeded STARTED");

                try
                {
                    while (true)
                    {
                        if (!needReconnect)
                        {
                            await Task.Delay(500, _tokenSource.Token);
                        }
                        else
                        {
                            logger.LogInformation("Streamlabs ReConnect needed.");
                            ReConnect();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                }

                logger.LogInformation("ListenReconnectNeeded STOPPED");
            });
        }

        private Task StartListenerTask(string socketToken)
        {
            return Task.Run(async () =>
            {
                var message = "";

                while (isConnected)
                {
                    if (Client.State == WebSocketState.Connecting)
                    {
                        await Task.Delay(200, _tokenSource.Token);
                        continue;
                    }

                    WebSocketReceiveResult result = null;
                    var buffer = new byte[1024 * 16];

                    try
                    {
                        result = await Client.ReceiveAsync(new ArraySegment<byte>(buffer), _tokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Streamlabs ReceiveAsync error.");
                        needReconnect = true;
                        break;
                    }

                    if (result == null) continue;

                    switch (result.MessageType)
                    {
                        case WebSocketMessageType.Close:
                            logger.LogWarning("Streamlabs channel close received");
                            needReconnect = true;
                            isConnected = false;
                            continue;
                        case WebSocketMessageType.Text when !result.EndOfMessage:
                            message += Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                            continue;
                        case WebSocketMessageType.Text:
                            message += Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                            // Defalut engine.io protocol
                            switch (buffer[0])
                            {
                                case (byte)'0': // Connected
                                    JsonDocument jdoc = JsonDocument.Parse(message[1..]);
                                    sessionId = jdoc.RootElement.GetProperty("sid").ToString();
                                    pingInterval = jdoc.RootElement.GetProperty("pingInterval").GetInt32();
                                    pingTimeout = jdoc.RootElement.GetProperty("pingTimeout").GetInt32();
                                    logger.LogInformation("sid:{0} pingInterval:{1} pingTimeout:{2}", sessionId, pingInterval, pingTimeout);
                                    break;
                                case (byte)'2': // Server Ping
                                    await Client.SendAsync(new ArraySegment<byte>(PONG_STRING), WebSocketMessageType.Text, true, CancellationToken.None);
                                    break;
                                case (byte)'3': // Server Pong
                                    pongReceived = true;
                                    break;
                                case (byte)'4': // Message
                                    logger.LogInformation("Streamlabs incoming message: {0}", message);
                                    switch (buffer[1])
                                    {
                                        case (byte)'0': // Connect
                                            pongTimer.Interval = pingTimeout;

                                            pingTimer.Interval = pingInterval - 5000;
                                            pingTimer.Start();
                                            break;
                                        case (byte)'1': // Disconnect
                                                        // Ignored
                                            break;
                                        case (byte)'2': // Event
                                            try
                                            {
                                                OnMessage?.Invoke(this, new OnMessageEventArgs(message));
                                            }
                                            catch (ArgumentException) { }
                                            break;
                                        case (byte)'3': // Ack
                                        case (byte)'4': // Error
                                        case (byte)'5': // Binary_Event
                                        case (byte)'6': // Binary_Ack
                                                        // Ignored
                                            break;
                                    }
                                    break;
                            }
                            break;
                        case WebSocketMessageType.Binary:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    message = "";
                }
            });
        }

        private void Close()
        {
            logger.LogInformation("Streamlabs socket Close started");

            isConnected = false;
            pingTimer.Stop();
            _tokenSource.Cancel();
            _tokenSource = new CancellationTokenSource();
            Client?.Abort();

            if (!(_networkTasks?.Length > 0)) return;
            if (Task.WaitAll(_networkTasks, 15000))
            {
                OnDisconnected?.Invoke(this, new OnDisconnectedEventArgs());
                return;
            }

            logger.LogInformation("Streamlabs socket Close failed.");

            OnFatality?.Invoke(this,
              new OnFatalErrorEventArgs
              {
                  Reason = "Fatal network error. Network services fail to shut down."
              });
        }

        private void ReConnect()
        {
            logger.LogInformation("Streamlabs Reconnect started");
            needReconnect = false;

            Close();
            Connect(socketToken);
            OnReconnected?.Invoke(this, new OnReconnectedEventArgs());
        }
    }
}

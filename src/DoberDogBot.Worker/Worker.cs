using Autofac;
using DoberDogBot.Application.Commands;
using DoberDogBot.Application.Models;
using DoberDogBot.Application.Queries;
using DoberDogBot.Infrastructure.AppDb;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using TwitchLib.Api;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Events;
using TwitchLib.Communication.Models;
using TwitchLib.PubSub;
using TwitchLib.PubSub.Events;

namespace DoberDogBot.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly BotOption _botOptions;
        private readonly TwitchOptions _twitchOption;
        private static TwitchClient _client;
        private static WebSocketClient customClient;
        private static System.Timers.Timer sleepTimer;
        private static System.Timers.Timer wakeTimer;
        private static System.Timers.Timer botTokenTimer;
        private static System.Timers.Timer chatterTimer;
        private static System.Timers.Timer streamAliveTimer;
        private static TwitchPubSub pubSubClient;
        private static ClientOptions clientOptions;
        private string botAuthToken;
        private static TwitchAPI api;
        private readonly IServiceScopeFactory _scopeFactory;
        private static int botId;
        private static string sessionId;

        public Worker(ILogger<Worker> logger,
            IOptions<TwitchOptions> twitchOption,
            IOptions<BotOption> botOptions,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _botOptions = botOptions.Value;
            _twitchOption = twitchOption.Value;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            sessionId = Guid.NewGuid().ToString("N");
            try
            {
                _logger.LogInformation($"Worker started! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

                #region StreamLabs Tips
                //try
                //{
                //    _logger.LogInformation($"Streamlabs init started! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

                //    var streamlabsSocketToken = await GetSocketToken(_twitchOption.ChannelId);

                //    streamlabsClient = new StreamlabsClient(_logger);
                //    streamlabsClient.OnDonationReceived += OnDonationReceived;
                //    streamlabsClient.Connect(streamlabsSocketToken);

                //    _logger.LogInformation($"Streamlabs init finished! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");
                //}
                //catch (Exception ex)
                //{
                //    _logger.LogError(ex, "Streamlabs client failed");
                //}
                #endregion

                clientOptions = new ClientOptions
                {
                    MessagesAllowedInPeriod = _twitchOption.MessagesAllowedInPeriod,
                    ThrottlingPeriod = TimeSpan.FromSeconds(_twitchOption.ThrottlingPeriodInSeconds)
                };

                customClient = new(clientOptions);

                botId = int.Parse(_twitchOption.ChannelId);

                api = new TwitchAPI();
                api.Settings.ClientId = _twitchOption.ClientId;
                api.Settings.Secret = _twitchOption.ClientSecret;

                var botAuthTokenResult = await GetAuthToken(_twitchOption.ChannelId);

                api.Settings.AccessToken = botAuthTokenResult.Key;
                botAuthToken = botAuthTokenResult.Key;

                var streamData = await api.Helix.Streams.GetStreamsAsync(userLogins: new List<string> { _twitchOption.Channel });

                await CreateCustomClient();

                //streamIsAlive
                if (streamData.Streams.Length > 0)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

                    var streamerQuery = scope.ServiceProvider.GetRequiredService<IStreamerQueries>();

                    var lastActiveSessionId = await streamerQuery.GetLastActiveSession(_twitchOption.ChannelId);

                    if (!string.IsNullOrEmpty(lastActiveSessionId))
                    {
                        sessionId = lastActiveSessionId;
                    }

                    await mediatr.Send(new StreamStarCommand { Channel = _twitchOption.Channel, TwitchClient = _client, BotId = botId, BotOption = _botOptions, SessionId = sessionId, PlayDelay = 0, StreamStartDate = streamData.Streams[0].StartedAt.ToString(CultureInfo.InvariantCulture) }, stoppingToken);

                    CreateIRCClient();
                }

                CreatePubSubClient();

                //3 hour refresh
                botTokenTimer = new System.Timers.Timer((1000 * botAuthTokenResult.Value) - (1000 * 60 * 30));
                botTokenTimer.Elapsed += ReconnectPubSub;
                botTokenTimer.AutoReset = false;
                botTokenTimer.Enabled = true;

                //every 3 minutes check stream status
                streamAliveTimer = new System.Timers.Timer(1000 * 60 * 3);
                streamAliveTimer.Elapsed += CheckStreamIsAlive;
                streamAliveTimer.AutoReset = true;
                streamAliveTimer.Enabled = true;

                await Task.Delay(-1, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, ex.Message);
            }
        }

        private void CreateIRCClient()
        {
            _logger.LogInformation($"CreateIRCClient started! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            _client.OnLog += Client_OnLog;
            _client.OnJoinedChannel += Client_OnJoinedChannel;
            _client.OnMessageReceived += Client_OnMessageReceived;
            _client.OnConnected += Client_OnConnected;
            _client.OnError += Client_Error;
            _client.OnDisconnected += Client_OnDisconnectedEvent;

            _client.Connect();

            if (_botOptions.AutoSleepEnabled)
                SetSleepTimer();

            if (_botOptions.PokeChattersEnabled)
                SetChattersTimer();
        }

        private async Task CreateCustomClient()
        {
            var chatToken = await GetAuthToken(_twitchOption.BotChannelId);

            ConnectionCredentials credentials = new(_twitchOption.BotName, chatToken.Key);

            _client = new TwitchClient(customClient);
            _client.Initialize(credentials, _twitchOption.Channel);
        }

        private void SetSleepTimer()
        {
            var interval = new Random().Next(_botOptions.MinSleepIntervalInMinutes * 1000, _botOptions.MaxSleepIntervalInMinutes * 1000) * 60;
            sleepTimer = new System.Timers.Timer(interval);
            sleepTimer.Elapsed += OnSleep;
            sleepTimer.AutoReset = false;
            sleepTimer.Enabled = true;
        }

        private void SetChattersTimer()
        {
            var interval = new Random().Next(_botOptions.MinChattersIntervalInMinutes * 1000, _botOptions.MaxChattersIntervalInMinutes * 1000) * 60;
            chatterTimer = new System.Timers.Timer(interval);
            chatterTimer.Elapsed += OnChatters;
            chatterTimer.AutoReset = true;
            chatterTimer.Enabled = true;
        }

        private void CloseIRCClient()
        {
            _logger.LogInformation($"CloseIRCClient started! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            try
            {
                sleepTimer?.Stop();
                wakeTimer?.Stop();
                chatterTimer?.Stop();

                if (_client != null)
                {
                    _client.Disconnect();

                    _client.OnLog -= Client_OnLog;
                    _client.OnJoinedChannel -= Client_OnJoinedChannel;
                    _client.OnMessageReceived -= Client_OnMessageReceived;
                    _client.OnConnected -= Client_OnConnected;
                    _client.OnError -= Client_Error;
                    _client.OnDisconnected -= Client_OnDisconnectedEvent;

                    _client = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseIRCClient");
            }

            _logger.LogInformation($"CloseIRCClient finished! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

        }

        private void CreatePubSubClient()
        {
            _logger.LogInformation($"CreatePubSubClient started! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            pubSubClient = new TwitchPubSub();

            pubSubClient.OnPubSubServiceConnected += OnPubSubServiceConnected;
            pubSubClient.OnListenResponse += OnListenResponse;
            pubSubClient.OnStreamUp += OnStreamUp;
            pubSubClient.OnStreamDown += OnStreamDown;
            pubSubClient.OnPubSubServiceError += OnPubSubServiceError;
            pubSubClient.OnBitsReceived += OnBitsReceived;
            pubSubClient.OnChannelSubscription += OnChannelSubscription;

            PubSubListenTopics();

            pubSubClient.Connect();

            _logger.LogInformation($"CreatePubSubClient finished! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");
        }

        private void ReConnectPubSubClient()
        {
            _logger.LogInformation($"ClosePubSubClient started! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            try
            {
                var botAuthTokenResult = GetAuthToken(_twitchOption.ChannelId).GetAwaiter().GetResult();
                botAuthToken = botAuthTokenResult.Key;

                PubSubListenTopics();
                pubSubClient.SendTopics(botAuthToken, true);
                PubSubListenTopics();
                pubSubClient.SendTopics(botAuthToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ClosePubSubClient");
            }

            _logger.LogInformation($"ClosePubSubClient finished! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");
        }

        private void PubSubListenTopics()
        {
            _logger.LogInformation($"Topics set! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            pubSubClient.ListenToVideoPlayback(_twitchOption.ChannelId);
            pubSubClient.ListenToBitsEvents(_twitchOption.ChannelId);
            pubSubClient.ListenToSubscriptions(_twitchOption.ChannelId);
        }

        #region IRC

        private void OnChatters(object source, ElapsedEventArgs e)
        {
            try
            {
                var chatters = api.Undocumented.GetChattersAsync(_twitchOption.Channel).GetAwaiter().GetResult();

                if (chatters != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();
                    mediatr.Send(new PokeChatterCommand { Channel = _twitchOption.Channel, TwitchClient = _client, Chatters = chatters.Select(x => x.Username).ToArray(), BotId = botId, BotOption = _botOptions, SessionId = sessionId }).GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OnChatters");
            }
        }

        private void OnSleep(object source, ElapsedEventArgs e)
        {
            var sleepDurationInMinutes = new Random().Next(_botOptions.MinSleepDurationInMinutes, _botOptions.MaxSleepDurationInMinutes);

            using var scope = _scopeFactory.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

            mediatr.Send(new AutoSleepCommand { Channel = _twitchOption.Channel, TwitchClient = _client, SleepDurationInMinutes = sleepDurationInMinutes, BotId = botId, BotOption = _botOptions, SessionId = sessionId }).GetAwaiter().GetResult();

            //set alarm to wakeup
            wakeTimer = new System.Timers.Timer(sleepDurationInMinutes * 1000 * 60);
            wakeTimer.Elapsed += OnWakeup;
            wakeTimer.AutoReset = false;
            wakeTimer.Enabled = true;
        }

        private void OnWakeup(object source, ElapsedEventArgs e)
        {
            sleepTimer.Interval = new Random().Next(_botOptions.MinSleepIntervalInMinutes * 1000, _botOptions.MaxSleepIntervalInMinutes * 1000) * 60;
            sleepTimer.Start();

            using var scope = _scopeFactory.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

            mediatr.Send(new AutoWakeCommand { Channel = _twitchOption.Channel, TwitchClient = _client, BotId = botId, BotOption = _botOptions, SessionId = sessionId }).GetAwaiter().GetResult();
        }

        private void Client_OnLog(object sender, TwitchLib.Client.Events.OnLogArgs e)
        {
            _logger.LogInformation($"{e.DateTime}: {e.BotUsername} - {e.Data}");
        }

        private void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            _logger.LogInformation($"Connected to {e.AutoJoinChannel}");
        }

        private void Client_OnJoinedChannel(object sender, OnJoinedChannelArgs e)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

            mediatr.Send(new BotOnJoinCommand { Channel = e.Channel, TwitchClient = _client, BotId = botId, BotOption = _botOptions, SessionId = sessionId }).GetAwaiter().GetResult();
        }

        private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            using var scope = _scopeFactory.CreateScope();
            var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();

            messageService.ProcessMessage(e.ChatMessage, _client, botId, sessionId).GetAwaiter().GetResult();
        }

        private void Client_Error(object sender, OnErrorEventArgs e)
        {
            _logger.LogError(e.Exception, "IRC Client_Error");
        }

        private void Client_OnDisconnectedEvent(object sender, OnDisconnectedEventArgs e)
        {
            _logger.LogError("IRC Disconnected");

        }

        #endregion

        #region PubSub

        private void OnPubSubServiceConnected(object sender, EventArgs e)
        {
            _logger.LogInformation($"OnPubSubServiceConnected! Server time: {DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)}");

            // SendTopics accepts an oauth optionally, which is necessary for some topics
            pubSubClient.SendTopics(botAuthToken);
        }

        private void OnListenResponse(object sender, OnListenResponseArgs e)
        {
            if (!e.Successful)
                throw new Exception($"Failed to listen! Response: {e.Response}");
        }

        private void OnStreamUp(object sender, OnStreamUpArgs e)
        {
            sessionId = Guid.NewGuid().ToString("N");
            _logger.LogInformation($"Stream just went up! Server time: {e.ServerTime} PlayDelay: {e.PlayDelay}");

            CloseIRCClient();

            CreateCustomClient().GetAwaiter().GetResult();

            CreateIRCClient();

            using var scope = _scopeFactory.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

            mediatr.Send(new StreamStarCommand { Channel = _twitchOption.Channel, TwitchClient = _client, BotId = botId, BotOption = _botOptions, SessionId = sessionId, PlayDelay = e.PlayDelay, StreamStartDate = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }).GetAwaiter().GetResult();
        }

        private void OnStreamDown(object sender, OnStreamDownArgs e)
        {
            _logger.LogInformation($"Stream just went down! Server time: {e.ServerTime}");

            using var scope = _scopeFactory.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

            mediatr.Send(new StreamEndCommand { Channel = _twitchOption.Channel, TwitchClient = _client, BotId = botId, BotOption = _botOptions, SessionId = sessionId, StreamEnded = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture) }).GetAwaiter().GetResult();

            CloseIRCClient();
        }

        private void OnChannelSubscription(object sender, OnChannelSubscriptionArgs e)
        {
            using var scope = _scopeFactory.CreateScope();
            var mediatr = scope.ServiceProvider.GetRequiredService<IMediator>();

            mediatr.Send(new SubscriberCommand
            {
                Channel = e.Subscription.ChannelName,
                TwitchClient = _client,
                BotId = botId,
                ChannelId = e.ChannelId,
                ChannelName = e.Subscription.ChannelName,
                Context = e.Subscription.Context,
                CumulativeMonths = e.Subscription.CumulativeMonths,
                DisplayName = e.Subscription.DisplayName,
                IsGift = e.Subscription.IsGift,
                Months = e.Subscription.Months,
                MultiMonthDuration = e.Subscription.MultiMonthDuration,
                RecipientDisplayName = e.Subscription.RecipientDisplayName,
                RecipientId = e.Subscription.RecipientId,
                RecipientName = e.Subscription.RecipientName,
                StreakMonths = e.Subscription.StreakMonths,
                SubMessage = e.Subscription.SubMessage.Message,
                SubscriptionPlan = e.Subscription.SubscriptionPlan.ToString(),
                SubscriptionPlanName = e.Subscription.SubscriptionPlanName,
                Time = e.Subscription.Time,
                UserId = e.Subscription.UserId,
                Username = e.Subscription.Username,
                BotOption = _botOptions,
                SessionId = sessionId
            }).GetAwaiter().GetResult();
        }

        private void OnBitsReceived(object sender, OnBitsReceivedArgs e)
        {
            //var isDonation = CheckIsDonation(e.ChatMessage);

            //using (var scope = _scopeFactory.CreateScope())
            //{
            //    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            //    dbContext.Bits.Add(new Bit
            //    {
            //        Id = Guid.NewGuid(),
            //        BitsUsed = e.BitsUsed,
            //        ChannelId = e.ChannelId,
            //        ChannelName = e.ChannelName,
            //        ChatMessage = e.ChatMessage,
            //        Context = e.Context,
            //        Time = e.Time,
            //        TotalBitsUsed = e.TotalBitsUsed,
            //        UserId = e.UserId,
            //        Username = e.Username,
            //        IsDonation = isDonation
            //    });

            //    dbContext.SaveChanges();
            //}

            //if (isDonation)
            //{
            //    _commandManager.Queue(new BitDonationCommand
            //    {
            //        Channel = e.ChannelName,
            //        TwitchClient = _client,
            //        DisplayName = e.Username,
            //        Bits = e.BitsUsed.ToString(), BotOptions = _botOptions
            //    });
            //}
        }

        private void OnPubSubServiceError(object sender, OnPubSubServiceErrorArgs e)
        {
            _logger.LogError(e.Exception, "PubSub OnPubSubServiceError");
        }

        #endregion

        private async Task<KeyValuePair<string, int>> GetAuthToken(string externalLoginId)
        {
            _logger.LogInformation($"Getting token for : {externalLoginId}");

            IdentityUserClaim<string> accessToken;
            IdentityUserClaim<string> refreshToken;
            string userId;

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var externalLogin = dbContext.UserLogins.Single(x => x.ProviderKey == externalLoginId);

                userId = externalLogin.UserId;

                var claims = dbContext.UserClaims.Where(x => x.UserId == userId);

                accessToken = claims.Single(x => x.ClaimType == "urn:twitch:access_token");
                refreshToken = claims.Single(x => x.ClaimType == "urn:twitch:refresh_token");
            }

            var newToken = await api.V5.Auth.RefreshAuthTokenAsync(refreshToken.ClaimValue, _twitchOption.ClientSecret);

            _logger.LogInformation($"NewToken acquired for : {externalLoginId}");

            accessToken.ClaimValue = newToken.AccessToken;
            refreshToken.ClaimValue = newToken.RefreshToken;

            using (var scope = _scopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                dbContext.UserClaims.Update(accessToken);
                dbContext.UserClaims.Update(refreshToken);

                await dbContext.SaveChangesAsync();
            }

            return new KeyValuePair<string, int>(newToken.AccessToken, newToken.ExpiresIn);
        }

        private void CheckStreamIsAlive(object source, ElapsedEventArgs e)
        {
            _logger.LogInformation($"CheckStreamIsAlive started.");

            var streamData = api.Helix.Streams.GetStreamsAsync(userLogins: new List<string> { _twitchOption.Channel }).GetAwaiter().GetResult();

            bool streamIsCurrentlyAlive = streamData.Streams.Length > 0;

            //try to reconnect
            if (streamIsCurrentlyAlive && (_client == null || !_client.IsConnected))
            {
                _logger.LogInformation($"CheckStreamIsAlive try to reconnect");
                CloseIRCClient();
                CreateCustomClient().GetAwaiter().GetResult();
                CreateIRCClient();
            }
            //disconnect
            else if (!streamIsCurrentlyAlive && (_client != null && _client.IsConnected))
            {
                _logger.LogInformation($"CheckStreamIsAlive disconnect");
                CloseIRCClient();
            }

            _logger.LogInformation($"CheckStreamIsAlive finished.");
        }

        private void ReconnectPubSub(object source, ElapsedEventArgs e)
        {
            _logger.LogInformation($"ReconnectPubSub");
            ReConnectPubSubClient();
            botTokenTimer.Start();
        }

        #region StreamlabsEvent
        //private void OnDonationReceived(object sender, DonationEventArgs e)
        //{
        //    var isDonation = CheckIsDonation(e.Donation.Message);

        //    using (var scope = _scopeFactory.CreateScope())
        //    {
        //        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //        dbContext.Tips.Add(new Tip
        //        {
        //            Id = Guid.NewGuid(),
        //            Amount = e.Donation.Amount.ToString(CultureInfo.InvariantCulture),
        //            Channel = e.Donation.To.Name,
        //            CreatedAt = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture),
        //            Name = e.Donation.Name,
        //            Message = e.Donation.Message,
        //            DonationId = e.Donation.Id.ToString(),
        //            Currency = e.Donation.DonationCurrency,
        //            IsDonation = isDonation
        //        });

        //        dbContext.SaveChanges();
        //    }

        //    if (isDonation)
        //    {
        //        _commandManager.Queue(new TipDonationCommand
        //        {
        //            Channel = _twitchOption.Channel,
        //            TwitchClient = _client,
        //            DisplayName = e.Donation.Name,
        //            Amount = $"{e.Donation.Amount:#0.00} {e.Donation.DonationCurrency}", BotOptions = _botOptions
        //        });
        //    }
        //}
        #endregion

        #region Streamlabs
        //private async Task<string> GetSocketToken(string externalLoginId)
        //{
        //    _logger.LogInformation($"Getting streamlabs socket token for : {externalLoginId}");

        //    string accesToken, soketToken;

        //    using (var scope = _scopeFactory.CreateScope())
        //    {
        //        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        //        var externalLogin = dbContext.UserLogins.Single(x => x.ProviderKey == externalLoginId);

        //        var claims = dbContext.UserClaims.Where(x => x.UserId == externalLogin.UserId);

        //        accesToken = claims.Single(x => x.ClaimType == "urn:streamlabs:access_token").ClaimValue;
        //    }

        //    http.DefaultRequestHeaders.Add("Authorization", "Bearer " + accesToken);
        //    var socketResponse = await http.GetStringAsync("https://streamlabs.com/api/v1.0/socket/token");

        //    var jDoc = JsonDocument.Parse(socketResponse);

        //    soketToken = jDoc.RootElement.GetProperty("socket_token").ToString();

        //    _logger.LogInformation($"New streamlabs socket token acquired for : {externalLoginId}");

        //    return soketToken;
        //}
        #endregion
    }
}

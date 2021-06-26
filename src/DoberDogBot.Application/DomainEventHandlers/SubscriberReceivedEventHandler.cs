using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class SubscriberReceivedEventHandler : BaseEventHandler<SubscriberReceivedEvent>
    {
        private readonly HttpClient _http;
        private readonly ILogger<SubscriberReceivedEventHandler> _logger;
        private readonly string _notifySubUri;
        public SubscriberReceivedEventHandler(ILogger<SubscriberReceivedEventHandler> logger, IHttpClientFactory clientFactory, IConfiguration configuration) : base(logger)
        {
            _logger = logger;
            _http = clientFactory.CreateClient();
            _notifySubUri = configuration.GetValue<string>("SUBSCRIBER_NOTIFICATION_URL");
        }

        public override async Task Handle(SubscriberReceivedEvent notification, CancellationToken cancellationToken)
        {
            await base.Handle(notification, cancellationToken);

            try
            {
                var uri = $"{_notifySubUri}?key={notification.ChannelId}&sessionId={notification.SessionId}";
                var res = await _http.GetAsync(uri, cancellationToken);

                if (!res.IsSuccessStatusCode)
                    _logger.LogError("----- SubscriberReceivedEventHandler {Uri} for {Code}", uri, res.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubscriberReceivedEventHandler");
            }
        }
    }
}
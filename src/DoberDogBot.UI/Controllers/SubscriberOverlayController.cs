using DoberDogBot.Application.Queries;
using DoberDogBot.UI.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Threading.Tasks;

namespace DoberDogBot.UI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SubscriberOverlayController : ControllerBase
    {
        private readonly IHubContext<TwitchSubHub> _hubContext;
        private readonly ILogger<SubscriberOverlayController> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SubscriberOverlayController(IHubContext<TwitchSubHub> hubContext,
            ILogger<SubscriberOverlayController> logger, IServiceScopeFactory scopeFactory)
        {
            _hubContext = hubContext;
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        [Route("update")]
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> UpdateSubCount(string channelId, string sessionId)
        {
            try
            {
                _logger.LogInformation("SubscriberOverlayController: {ChannelId} {SessionId} ", channelId, sessionId);

                using var scope = _scopeFactory.CreateScope();

                var subscriberQueries = scope.ServiceProvider.GetRequiredService<ISubscriberQueries>();

                var subCount = await subscriberQueries.GetDailySubCount(channelId, sessionId);

                await _hubContext.Clients.Group(channelId).SendAsync("ReceiveMessage", subCount);

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SubscriberOverlayController: {ChannelId} {SessionId} ", channelId, sessionId);

                throw;
            }
        }
    }
}

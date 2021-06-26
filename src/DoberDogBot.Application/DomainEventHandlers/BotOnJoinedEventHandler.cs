using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class BotOnJoinedEventHandler : BaseEventHandler<BotOnJoinedEvent>
    {
        public BotOnJoinedEventHandler(ILogger<BotOnJoinedEventHandler> logger) : base(logger)
        {
        }
    }
}

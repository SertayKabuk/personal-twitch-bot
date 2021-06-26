using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class BroadcasterAttackToSelfEventHandler : BaseEventHandler<BroadcasterAttackToSelfEvent>
    {
        public BroadcasterAttackToSelfEventHandler(ILogger<BroadcasterAttackToSelfEventHandler> logger) : base(logger)
        {
        }
    }
}

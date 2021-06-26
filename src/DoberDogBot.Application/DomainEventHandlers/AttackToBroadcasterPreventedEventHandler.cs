using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class AttackToBroadcasterPreventedEventHandler : BaseEventHandler<AttackToBroadcasterPreventedEvent>
    {
        public AttackToBroadcasterPreventedEventHandler(ILogger<AttackToBroadcasterPreventedEventHandler> logger) : base(logger)
        {
        }
    }
}

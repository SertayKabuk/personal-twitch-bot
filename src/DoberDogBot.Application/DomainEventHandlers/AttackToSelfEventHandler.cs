using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class AttackToSelfEventHandler : BaseEventHandler<AttackToSelfEvent>
    {
        public AttackToSelfEventHandler(ILogger<AttackToSelfEventHandler> logger) : base(logger)
        {
        }
    }
}

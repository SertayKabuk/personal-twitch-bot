using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class WakeAndAttacEventHandler : BaseEventHandler<WakeAndAttacEvent>
    {
        public WakeAndAttacEventHandler(ILogger<WakeAndAttacEventHandler> logger) : base(logger)
        {
        }
    }
}
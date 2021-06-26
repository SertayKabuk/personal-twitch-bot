using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class WakeAndProtecEventHandler : BaseEventHandler<WakeAndProtecEvent>
    {
        public WakeAndProtecEventHandler(ILogger<WakeAndProtecEventHandler> logger) : base(logger)
        {
        }
    }
}
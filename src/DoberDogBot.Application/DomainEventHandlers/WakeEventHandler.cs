using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class WakeEventHandler : BaseEventHandler<WakeEvent>
    {
        public WakeEventHandler(ILogger<WakeEventHandler> logger) : base(logger)
        {
        }
    }
}
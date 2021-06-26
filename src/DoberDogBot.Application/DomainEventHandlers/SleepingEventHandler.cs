using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class SleepingEventHandler : BaseEventHandler<SleepingEvent>
    {
        public SleepingEventHandler(ILogger<SleepingEventHandler> logger) : base(logger)
        {
        }
    }
}
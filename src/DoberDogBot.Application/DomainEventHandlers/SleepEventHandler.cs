using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class SleepEventHandler : BaseEventHandler<SleepEvent>
    {
        public SleepEventHandler(ILogger<SleepEventHandler> logger) : base(logger)
        {
        }
    }
}
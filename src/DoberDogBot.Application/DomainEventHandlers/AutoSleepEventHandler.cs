using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class AutoSleepEventHandler : BaseEventHandler<AutoSleepEvent>
    {
        public AutoSleepEventHandler(ILogger<AutoSleepEventHandler> logger) : base(logger)
        {
        }
    }
}
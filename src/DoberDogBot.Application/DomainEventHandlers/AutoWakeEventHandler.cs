using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class AutoWakeEventHandler : BaseEventHandler<AutoWakeEvent>
    {
        public AutoWakeEventHandler(ILogger<AutoWakeEventHandler> logger) : base(logger)
        {
        }
    }
}
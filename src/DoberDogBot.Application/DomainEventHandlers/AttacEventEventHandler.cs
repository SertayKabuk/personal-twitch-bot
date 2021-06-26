using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class AttacEventEventHandler : BaseEventHandler<AttacEvent>
    {
        public AttacEventEventHandler(ILogger<AttacEventEventHandler> logger) : base(logger)
        {
        }
    }
}

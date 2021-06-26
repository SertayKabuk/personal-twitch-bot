using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class ProtecEventHandler : BaseEventHandler<ProtecEvent>
    {
        public ProtecEventHandler(ILogger<ProtecEventHandler> logger) : base(logger)
        {
        }
    }
}
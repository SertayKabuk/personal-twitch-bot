using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class PokeChatterEventHandler : BaseEventHandler<PokeChatterEvent>
    {
        public PokeChatterEventHandler(ILogger<PokeChatterEventHandler> logger) : base(logger)
        {
        }
    }
}
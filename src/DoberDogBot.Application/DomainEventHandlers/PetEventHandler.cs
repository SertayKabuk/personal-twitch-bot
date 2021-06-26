using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class PetEventHandler : BaseEventHandler<PetEvent>
    {
        public PetEventHandler(ILogger<PetEventHandler> logger) : base(logger)
        {
        }
    }
}
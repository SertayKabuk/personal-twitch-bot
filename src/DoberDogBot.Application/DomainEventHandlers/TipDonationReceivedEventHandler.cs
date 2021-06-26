using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class TipDonationReceivedEventHandler : BaseEventHandler<TipDonationReceivedEvent>
    {
        public TipDonationReceivedEventHandler(ILogger<TipDonationReceivedEventHandler> logger) : base(logger)
        {
        }
    }
}
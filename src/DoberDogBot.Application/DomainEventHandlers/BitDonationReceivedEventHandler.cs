using DoberDogBot.Domain.Events;
using Microsoft.Extensions.Logging;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public class BitDonationReceivedEventHandler : BaseEventHandler<BitDonationReceivedEvent>
    {
        public BitDonationReceivedEventHandler(ILogger<BitDonationReceivedEventHandler> logger) : base(logger)
        {
        }
    }
}
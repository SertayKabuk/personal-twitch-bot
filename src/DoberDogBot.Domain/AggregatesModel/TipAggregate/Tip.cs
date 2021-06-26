using DoberDogBot.Domain.SeedWork;

namespace DoberDogBot.Domain.AggregatesModel.TipAggregate
{
    public class Tip : Entity, IAggregateRoot
    {
        public Tip(string donationId, string channel, string createdAt, string currency, string amount, string name, string message, bool isDonation, string sessionId)
        {
            DonationId = donationId;
            Channel = channel;
            CreatedAt = createdAt;
            Currency = currency;
            Amount = amount;
            Name = name;
            Message = message;
            IsDonation = isDonation;
            SessionId = sessionId;
        }

        public string DonationId { get; private set; }
        public string Channel { get; private set; }
        public string CreatedAt { get; private set; }
        public string Currency { get; private set; }
        public string Amount { get; private set; }
        public string Name { get; private set; }
        public string Message { get; private set; }
        public bool IsDonation { get; private set; }
        public string SessionId { get; private set; }
    }
}

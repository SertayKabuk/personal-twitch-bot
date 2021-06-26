using DoberDogBot.Domain.SeedWork;

namespace DoberDogBot.Domain.AggregatesModel.BitAggregate
{
    public class Bit : Entity, IAggregateRoot
    {
        public Bit(string username, string channelName, string userId, string channelId, string time, string chatMessage, int bitsUsed, int totalBitsUsed, string context, bool isDonation, string sessionId)
        {
            Username = username;
            ChannelName = channelName;
            UserId = userId;
            ChannelId = channelId;
            Time = time;
            ChatMessage = chatMessage;
            BitsUsed = bitsUsed;
            TotalBitsUsed = totalBitsUsed;
            Context = context;
            IsDonation = isDonation;
            SessionId = sessionId;
        }

        public string Username { get; private set; }
        public string ChannelName { get; private set; }
        public string UserId { get; private set; }
        public string ChannelId { get; private set; }
        public string Time { get; private set; }
        public string ChatMessage { get; private set; }
        public int BitsUsed { get; private set; }
        public int TotalBitsUsed { get; private set; }
        public string Context { get; private set; }
        public bool IsDonation { get; private set; }
        public string SessionId { get; private set; }
    }
}

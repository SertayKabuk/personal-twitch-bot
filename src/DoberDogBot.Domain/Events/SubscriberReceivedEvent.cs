namespace DoberDogBot.Domain.Events
{
    public class SubscriberReceivedEvent : BaseEvent
    {
        public string ChannelId { get; set; }
        public string SessionId { get; set; }
    }
}
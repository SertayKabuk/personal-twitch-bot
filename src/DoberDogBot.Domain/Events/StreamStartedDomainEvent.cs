namespace DoberDogBot.Domain.Events
{
    public class StreamStartedDomainEvent : BaseEvent
    {
        public string ChannelId { get; set; }
        public string SessionId { get; set; }
    }
}

using MediatR;
using TwitchLib.Client;

namespace DoberDogBot.Domain.Events
{
    public class BaseEvent : INotification
    {
        public TwitchClient TwitchClient { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public DomainResult DomainResult { get; set; }
    }
}

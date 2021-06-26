using TwitchLib.Client.Models;

namespace DoberDogBot.Domain.Commands
{
    public record WakeDomainCommand : BaseDomainCommand
    {
        public ChatMessage ChatMessage { get; set; }
    }
}

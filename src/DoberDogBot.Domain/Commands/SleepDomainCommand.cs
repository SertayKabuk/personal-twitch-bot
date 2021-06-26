using TwitchLib.Client.Models;

namespace DoberDogBot.Domain.Commands
{
    public record SleepDomainCommand : BaseDomainCommand
    {
        public ChatMessage ChatMessage { get; set; }
    }
}

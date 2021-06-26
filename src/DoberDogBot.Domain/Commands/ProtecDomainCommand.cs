using TwitchLib.Client.Models;

namespace DoberDogBot.Domain.Commands
{
    public record ProtecDomainCommand : BaseDomainCommand
    {
        public ChatMessage ChatMessage { get; set; }
    }
}

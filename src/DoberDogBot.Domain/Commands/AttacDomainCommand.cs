using TwitchLib.Client.Models;

namespace DoberDogBot.Domain.Commands
{
    public record AttacDomainCommand : BaseDomainCommand
    {
        public ChatMessage ChatMessage { get; set; }
    }
}

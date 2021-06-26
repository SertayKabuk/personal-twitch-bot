using TwitchLib.Client.Models;

namespace DoberDogBot.Domain.Commands
{
    public record PetDomainCommand : BaseDomainCommand
    {
        public ChatMessage ChatMessage { get; set; }

    }
}

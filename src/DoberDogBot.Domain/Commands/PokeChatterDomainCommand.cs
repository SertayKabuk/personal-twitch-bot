namespace DoberDogBot.Domain.Commands
{
    public record PokeChatterDomainCommand : BaseDomainCommand
    {
        public string[] Chatters { get; set; }
    }
}

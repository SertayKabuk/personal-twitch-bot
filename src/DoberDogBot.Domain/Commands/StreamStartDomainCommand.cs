namespace DoberDogBot.Domain.Commands
{
    public record StreamStartDomainCommand : BaseDomainCommand
    {
        public int PlayDelay { get; set; }
        public string StreamStartDate { get; set; }
    }
}

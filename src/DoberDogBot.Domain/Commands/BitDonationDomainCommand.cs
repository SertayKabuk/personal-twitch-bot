namespace DoberDogBot.Domain.Commands
{
    public record BitDonationDomainCommand : BaseDomainCommand
    {
        public string DisplayName { get; set; }
        public string Bits { get; set; }
    }
}

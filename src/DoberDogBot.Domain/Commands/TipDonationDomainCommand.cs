namespace DoberDogBot.Domain.Commands
{
    public record TipDonationDomainCommand : BaseDomainCommand
    {
        public string DisplayName { get; set; }
        public string Amount { get; set; }
    }
}

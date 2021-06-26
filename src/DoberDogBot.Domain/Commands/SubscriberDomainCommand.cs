namespace DoberDogBot.Domain.Commands
{
    public record SubscriberDomainCommand : BaseDomainCommand
    {
        public string DisplayName { get; set; }
        public string Amount { get; set; }
    }
}

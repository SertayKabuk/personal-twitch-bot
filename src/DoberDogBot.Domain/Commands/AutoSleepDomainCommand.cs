namespace DoberDogBot.Domain.Commands
{
    public record AutoSleepDomainCommand : BaseDomainCommand
    {
        public int SleepDurationInMinutes { get; set; }
    }
}

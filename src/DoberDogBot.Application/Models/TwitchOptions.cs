namespace DoberDogBot.Application.Models
{
    public class TwitchOptions
    {
        public const string OptionsName = "Twitch";

        public string BotName { get; set; }
        public string BotChannelId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Channel { get; set; }
        public string ChannelId { get; set; }
        public string DonationKeyword { get; set; }
        public int MessagesAllowedInPeriod { get; set; }
        public int ThrottlingPeriodInSeconds { get; set; }
    }
}

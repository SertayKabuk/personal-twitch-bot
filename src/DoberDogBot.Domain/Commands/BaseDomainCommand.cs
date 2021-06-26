using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using TwitchLib.Client;

namespace DoberDogBot.Domain.Commands
{
    public record BaseDomainCommand
    {
        public TwitchClient TwitchClient { get; set; }
        public string Channel { get; set; }
        public string CommandName { get; set; }
        public BotOption BotOption { get; set; }
        public int BotId { get; set; }
        public string SessionId { get; set; }
    }
}

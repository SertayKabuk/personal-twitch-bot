using AutoMapper;

namespace DoberDogBot.Application.Models
{
    public class BotOption
    {
        public const string OptionsName = "Bot";
        public string BotName { get; set; }
        public BotCommand[] BotCommands { get; set; }
        public bool AutoSleepEnabled { get; set; }
        public bool PokeChattersEnabled { get; set; }

        public int MinSleepIntervalInMinutes { get; set; }
        public int MaxSleepIntervalInMinutes { get; set; }

        public int MinSleepDurationInMinutes { get; set; }
        public int MaxSleepDurationInMinutes { get; set; }

        public int MinChattersIntervalInMinutes { get; set; }
        public int MaxChattersIntervalInMinutes { get; set; }
        public string[] KnownBotNames { get; set; }
    }

    public class BotOptionsProfile : Profile
    {
        public BotOptionsProfile()
        {
            CreateMap<BotOption, Domain.AggregatesModel.BotAggregate.BotOption>();
        }
    }

    public class BotCommand
    {
        public string Command { get; set; }
        public bool AllowMods { get; set; }
        public bool AllowEveryone { get; set; }
        public BotEvent[] BotEvents { get; set; }
    }

    public class BotCommandProfile : Profile
    {
        public BotCommandProfile()
        {
            CreateMap<BotCommand, Domain.AggregatesModel.BotAggregate.BotCommand>();
        }
    }

    public class BotEvent
    {
        public string Event { get; set; }
        public string[] SuccessMessages { get; set; }
        public string[] FailMessages { get; set; }
        public string[] IdempotentMessages { get; set; }
        public string[] NotMommyMessages { get; set; }
    }

    public class BotEventProfile : Profile
    {
        public BotEventProfile()
        {
            CreateMap<BotEvent, Domain.AggregatesModel.BotAggregate.BotEvent> ();
        }
    }
}

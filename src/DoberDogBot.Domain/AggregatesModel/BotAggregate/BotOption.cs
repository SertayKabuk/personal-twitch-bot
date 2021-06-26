using DoberDogBot.Domain.SeedWork;
using System.Collections.Generic;

namespace DoberDogBot.Domain.AggregatesModel.BotAggregate
{
    public class BotOption : ValueObject
    {
        public string BotName { get; set; }
        public string[] KnownBotNames { get; set; }
        public BotCommand[] BotCommands { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return BotName;
            yield return KnownBotNames;
            yield return BotCommands;
        }
    }
}

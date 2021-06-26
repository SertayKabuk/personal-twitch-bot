using DoberDogBot.Domain.SeedWork;
using System.Collections.Generic;

namespace DoberDogBot.Domain.AggregatesModel.BotAggregate
{
    public class BotCommand : ValueObject
    {
        public string Command { get; set; }
        public bool AllowMods { get; set; }
        public bool AllowEveryone { get; set; }
        public BotEvent[] BotEvents { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Command;
            yield return AllowMods;
            yield return AllowEveryone;
            yield return BotEvents;
        }
    }
}

using DoberDogBot.Domain.SeedWork;
using System.Collections.Generic;

namespace DoberDogBot.Domain.AggregatesModel.BotAggregate
{
    public class BotEvent : ValueObject
    {
        public string Event { get; set; }
        public string[] SuccessMessages { get; set; }
        public string[] FailMessages { get; set; }
        public string[] IdempotentMessages { get; set; }
        public string[] NotMommyMessages { get; set; }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Event;
            yield return SuccessMessages;
            yield return FailMessages;
            yield return IdempotentMessages;
            yield return NotMommyMessages;
        }
    }
}

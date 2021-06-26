using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Events;
using System;

namespace DoberDogBot.Domain.Extensions
{
    public static class BotEventExtentions
    {
        public static string GetResponseMessage(this BotEvent botEvent, DomainResult domainResult)
        {
            string[] messages = Array.Empty<string>();

            if (domainResult == DomainResult.Fail)
            {
                messages = botEvent.FailMessages;
            }
            else if (domainResult == DomainResult.Idempotent)
            {
                messages = botEvent.IdempotentMessages;
            }
            else if (domainResult == DomainResult.NotMommy)
            {
                messages = botEvent.NotMommyMessages;
            }
            else if (domainResult == DomainResult.Success)
            {
                messages = botEvent.SuccessMessages;
            }

            if (messages.Length > 0)
            {
                return messages[new Random().Next(messages.Length)];
            }

            return string.Empty;
        }
    }
}

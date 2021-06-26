using DoberDogBot.Domain.Commands;
using DoberDogBot.Domain.Events;
using DoberDogBot.Domain.Extensions;
using DoberDogBot.Domain.SeedWork;
using System;
using System.Linq;

namespace DoberDogBot.Domain.AggregatesModel.SubscriberAggregate
{
    public class Subscriber : Entity, IAggregateRoot
    {
        public string ChannelId { get; private set; }
        public string SubMessage { get; private set; }
        public string Context { get; private set; }
        public int? StreakMonths { get; private set; }
        public int? CumulativeMonths { get; private set; }
        public int? Months { get; private set; }
        public string SubscriptionPlanName { get; private set; }
        public string SubscriptionPlan { get; private set; }
        public bool? IsGift { get; private set; }
        public DateTime Time { get; private set; }
        public string UserId { get; private set; }
        public string ChannelName { get; private set; }
        public string RecipientDisplayName { get; private set; }
        public string RecipientName { get; private set; }
        public string DisplayName { get; private set; }
        public string Username { get; private set; }
        public string RecipientId { get; private set; }
        public int? MultiMonthDuration { get; private set; }
        public bool IsDonation { get; private set; }
        public string SessionId { get; private set; }

        protected Subscriber() { }

        public Subscriber(string channelId, string subMessage, string context, int? streakMonths, int? cumulativeMonths, int? months, string subscriptionPlanName, string subscriptionPlan, bool? isGift, DateTime time, string userId, string channelName, string recipientDisplayName, string recipientName, string displayName, string username, string recipientId, int? multiMonthDuration, bool isDonation, string sessionId)
        {
            ChannelId = channelId;
            SubMessage = subMessage;
            Context = context;
            StreakMonths = streakMonths;
            CumulativeMonths = cumulativeMonths;
            Months = months;
            SubscriptionPlanName = subscriptionPlanName;
            SubscriptionPlan = subscriptionPlan;
            IsGift = isGift;
            Time = time;
            UserId = userId;
            ChannelName = channelName;
            RecipientDisplayName = recipientDisplayName;
            RecipientName = recipientName;
            DisplayName = displayName;
            Username = username;
            RecipientId = recipientId;
            MultiMonthDuration = multiMonthDuration;
            IsDonation = isDonation;
            SessionId = sessionId;
        }

        public void SubscriberReceived(SubscriberDomainCommand command)
        {
            var domainResult = DomainResult.Success;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            BaseEvent @event = new SubscriberReceivedEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult, ChannelId = command.BotId.ToString(), SessionId = SessionId };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{DisplayName}", command.DisplayName).Replace("{Amount}", command.Amount);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }
    }
}

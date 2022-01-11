using DoberDogBot.Domain.Commands;
using DoberDogBot.Domain.Events;
using DoberDogBot.Domain.Extensions;
using DoberDogBot.Domain.SeedWork;
using System;
using System.Linq;

namespace DoberDogBot.Domain.AggregatesModel.BitAggregate
{
    public class Bit : Entity, IAggregateRoot
    {
        public Bit(string username, string channelName, string userId, string channelId, string time, string chatMessage, int bitsUsed, int totalBitsUsed, string context, bool isDonation, string sessionId)
        {
            Username = username;
            ChannelName = channelName;
            UserId = userId;
            ChannelId = channelId;
            Time = time;
            ChatMessage = chatMessage;
            BitsUsed = bitsUsed;
            TotalBitsUsed = totalBitsUsed;
            Context = context;
            IsDonation = isDonation;
            SessionId = sessionId;
        }

        public string Username { get; private set; }
        public string ChannelName { get; private set; }
        public string UserId { get; private set; }
        public string ChannelId { get; private set; }
        public string Time { get; private set; }
        public string ChatMessage { get; private set; }
        public int BitsUsed { get; private set; }
        public int TotalBitsUsed { get; private set; }
        public string Context { get; private set; }
        public bool IsDonation { get; private set; }
        public string SessionId { get; private set; }

        public void BitReceived(BitDonationDomainCommand command)
        {
            var domainResult = DomainResult.Success;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            BaseEvent @event = new BitDonationReceivedEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{DisplayName}", command.DisplayName).Replace("{Amount}", command.Bits);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }
    }
}

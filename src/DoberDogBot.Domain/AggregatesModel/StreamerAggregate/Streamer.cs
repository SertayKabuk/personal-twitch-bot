using DoberDogBot.Domain.Commands;
using DoberDogBot.Domain.Events;
using DoberDogBot.Domain.Exceptions;
using DoberDogBot.Domain.SeedWork;
using System.Collections.Generic;
using System.Linq;

namespace DoberDogBot.Domain.AggregatesModel.StreamerAggregate
{
    public class Streamer : Entity, IAggregateRoot
    {
        private readonly List<StreamerSession> _streamerSessions;
        public IReadOnlyCollection<StreamerSession> StreamerSessions => _streamerSessions;

        private string _channel;
        private string _channelId;

        public string Channel => _channel;
        public string ChannelId => _channelId;

        protected Streamer()
        {
            _streamerSessions = new();
        }

        public Streamer(string channelId, string channel) : this()
        {
            _channelId = channelId;
            _channel = channel;
        }

        public void AddSession(StreamStartDomainCommand command)
        {
            var existingSession = _streamerSessions.Where(o => o.SessionId == command.SessionId).SingleOrDefault();

            if (existingSession == null)
            {
                var session = new StreamerSession(command.SessionId, command.PlayDelay, command.StreamStartDate, null);
                _streamerSessions.Add(session);
            }

            BaseEvent @event = new StreamStartedDomainEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = DomainResult.Success, ChannelId = command.BotId.ToString(), SessionId = command.SessionId };
            AddDomainEvent(@event);
        }

        public void SetSessionEndDate(string sessionId, string streamEnd)
        {
            var existingSession = _streamerSessions.Where(o => o.SessionId == sessionId).SingleOrDefault();

            if (existingSession == null)
            {
                throw new StreamerDomainException($"Session not found: {sessionId}");
            }

            existingSession.SetStreamEndDate(streamEnd);
        }
    }
}
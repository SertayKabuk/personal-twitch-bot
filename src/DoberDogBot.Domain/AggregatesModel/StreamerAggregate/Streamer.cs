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

        protected Streamer()
        {
            _streamerSessions = new();
        }

        public Streamer(string channel, string channelId) : this()
        {
            _channel = channel;
            _channelId = channelId;
        }

        public void AddSession(string sessionId, int playDelay, string streamStart, string streamEnd)
        {
            var existingSession = _streamerSessions.Where(o => o.SessionId == sessionId).SingleOrDefault();

            if (existingSession == null)
            {
                var session = new StreamerSession(sessionId, playDelay, streamStart, streamEnd);
                _streamerSessions.Add(session);
            }
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
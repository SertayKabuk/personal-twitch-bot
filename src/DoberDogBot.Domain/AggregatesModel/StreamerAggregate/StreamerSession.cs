using DoberDogBot.Domain.SeedWork;

namespace DoberDogBot.Domain.AggregatesModel.StreamerAggregate
{
    public class StreamerSession : Entity
    {
        public string SessionId { get; private set; }

        private int _playDelay;
        private string _streamStart;
        private string _streamEnd;

        protected StreamerSession() { }

        public StreamerSession(string sessionId, int playDelay, string streamStart, string streamEnd)
        {
            SessionId = sessionId;
            _playDelay = playDelay;
            _streamStart = streamStart;
            _streamEnd = streamEnd;
        }

        public string GetStreamStartDate() => _streamStart;
        public string GetStreamEndDate() => _streamEnd;

        public void SetStreamEndDate(string streamEnd)
        {
            _streamEnd = streamEnd;
        }
    }
}
using DoberDogBot.Domain.SeedWork;
using System;

namespace DoberDogBot.Domain.AggregatesModel.BotAggregate
{
    public class Ban : Entity
    {
        public string Username { get; private set; }
        private DateTime _banDate;

        protected Ban() { }

        public Ban(string username, DateTime banDate)
        {
            Username = username;
            _banDate = banDate;
        }

        public DateTime GetBanDate() => _banDate;
    }
}

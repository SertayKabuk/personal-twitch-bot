using DoberDogBot.Domain.Commands;
using DoberDogBot.Domain.Events;
using DoberDogBot.Domain.Extensions;
using DoberDogBot.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DoberDogBot.Domain.AggregatesModel.BotAggregate
{
    public class Bot : Entity, IAggregateRoot
    {
        private DateTime? _lastSleepTime;
        private DateTime? _wakeupTime;
        private bool _wakeLock;
        private string _lastPokedChatterDisplayName;

        private readonly List<Ban> _bans;
        public IReadOnlyCollection<Ban> Bans => _bans;

        private int getRandomChatterNameIteration = 0;

        protected Bot()
        {
            _bans = new();
        }

        public Bot(int id, DateTime? lastSleepTime, DateTime? wakeupTime, bool wakeLock, string lastPokedChatterDisplayName) : this()
        {
            Id = id;
            _lastSleepTime = lastSleepTime;
            _wakeupTime = wakeupTime;
            _wakeLock = wakeLock;
            _lastPokedChatterDisplayName = lastPokedChatterDisplayName;
        }

        public void AddBan(string username, DateTime banDate)
        {
            var existingBan = _bans.Where(o => o.Username == username).SingleOrDefault();

            if (existingBan == null)
            {
                var ban = new Ban(username, banDate);
                _bans.Add(ban);
            }
        }

        public void PokeChatter(PokeChatterDomainCommand command)
        {
            var domainResult = DomainResult.Success;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            BaseEvent @event = new PokeChatterEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            getRandomChatterNameIteration = 0;

            string chatterName = GetRandomChatterName(command.Chatters, command.BotOption.KnownBotNames);

            if (string.IsNullOrEmpty(chatterName) || !(CheckIfBotIsAwake(command.Channel, out bool didIWakeup, false)))
                @event.DomainResult = DomainResult.Ignored;

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{DisplayName}", chatterName);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void BitDonation(BitDonationDomainCommand command)
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



        public void TipDonation(TipDonationDomainCommand command)
        {
            var domainResult = DomainResult.Success;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            BaseEvent @event = new TipDonationReceivedEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{DisplayName}", command.DisplayName).Replace("{Amount}", command.Amount);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void Attac(AttacDomainCommand command)
        {
            var domainResult = DomainResult.Fail;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            bool sleeping = false;

            if (CheckIfBotIsAwake(command.Channel, out bool didIWakeup, command.ChatMessage.IsBroadcaster))
            {
                if (CheckCommandAuth(command.ChatMessage.IsBroadcaster, command.ChatMessage.IsModerator, commandOption))
                    domainResult = DomainResult.Success;
                else
                    domainResult = DomainResult.NotMommy;
            }
            else
            {
                sleeping = true;
                domainResult = DomainResult.Success;
            }

            var targetUser = command.ChatMessage.GetTargetUserName().Trim();

            BaseEvent @event = didIWakeup ? new WakeAndAttacEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult } :
                 new AttacEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            if (sleeping)
                @event = new SleepingEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            if (_bans.Any(x => x.Username == command.ChatMessage.DisplayName.ToLower()))
                domainResult = DomainResult.Ignored;

            //check if target is broadcaster
            if ((domainResult == DomainResult.Success || domainResult == DomainResult.NotMommy) && command.Channel.ToLower() == targetUser.ToLower())
            {
                if (!command.ChatMessage.IsBroadcaster)
                {
                    AddBan(command.ChatMessage.DisplayName.ToLower(), DateTime.UtcNow);

                    @event = new AttackToBroadcasterPreventedEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };
                }
                else
                {
                    @event = new BroadcasterAttackToSelfEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };
                }
            }
            //check if target is bot
            else if ((domainResult == DomainResult.Success) && command.BotOption.BotName.ToLower() == targetUser.ToLower())
            {
                @event = new AttackToSelfEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };
            }

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{TargetUser}", targetUser).Replace("{DisplayName}", command.ChatMessage.DisplayName);

            if (string.IsNullOrEmpty(targetUser))
                @event.DomainResult = DomainResult.Ignored;

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void Protec(ProtecDomainCommand command)
        {
            var domainResult = DomainResult.Fail;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            bool sleeping = false;

            if (CheckIfBotIsAwake(command.Channel, out bool didIWakeup, command.ChatMessage.IsBroadcaster))
            {
                if (CheckCommandAuth(command.ChatMessage.IsBroadcaster, command.ChatMessage.IsModerator, commandOption))
                    domainResult = DomainResult.Success;
                else
                    domainResult = DomainResult.NotMommy;
            }
            else
            {
                sleeping = true;
                domainResult = DomainResult.Success;

            }

            var targetUser = command.ChatMessage.GetTargetUserName().Trim();

            var banRemoveList = _bans.Where(x => x.Username == targetUser.ToLower());

            //if broadcaster protec someone in asslist, target will be removed from the list
            if (command.ChatMessage.IsBroadcaster && banRemoveList.Any())
            {
                foreach (var ban in banRemoveList)
                {
                    _bans.Remove(ban);
                }
            }

            if (_bans.Any(x => x.Username == command.ChatMessage.DisplayName.ToLower()))
                domainResult = DomainResult.Ignored;

            BaseEvent @event = didIWakeup ? new WakeAndProtecEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult } :
                 new ProtecEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            if (sleeping)
                @event = new SleepingEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);


            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{TargetUser}", targetUser).Replace("{DisplayName}", command.ChatMessage.DisplayName);

            if (string.IsNullOrEmpty(targetUser))
                @event.DomainResult = DomainResult.Ignored;

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void BotOnJoin(BotOnJoinDomainCommand command)
        {
            _bans.Clear();
            _lastSleepTime = null;
            _wakeupTime = DateTime.UtcNow;
            _wakeLock = false;
            _lastPokedChatterDisplayName = string.Empty;
            getRandomChatterNameIteration = 0;

            var domainResult = DomainResult.Success;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            BaseEvent @event = new BotOnJoinedEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{Channel}", command.Channel);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void AutoSleep(AutoSleepDomainCommand command)
        {
            _lastSleepTime = DateTime.UtcNow;
            _wakeupTime = DateTime.UtcNow.AddMinutes(command.SleepDurationInMinutes);

            var domainResult = DomainResult.Success;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            BaseEvent @event = new AutoSleepEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{Channel}", command.Channel);

            if (!CheckIfBotIsAwake(command.Channel, out _))
            {
                @event.DomainResult = DomainResult.Ignored;
            }

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void AutoWake(AutoWakeDomainCommand command)
        {
            _lastSleepTime = null;
            _wakeupTime = DateTime.UtcNow;

            var domainResult = DomainResult.Success;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            BaseEvent @event = new AutoWakeEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{Channel}", command.Channel);

            if (CheckIfBotIsAwake(command.Channel, out _))
            {
                @event.DomainResult = DomainResult.Ignored;
            }

            //locked manually, must call !wake command to wake
            if (_wakeLock)
                @event.DomainResult = DomainResult.Ignored;

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void Pet(PetDomainCommand command)
        {
            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            var domainResult = DomainResult.Success;

            bool sleeping = false;

            if (!CheckIfBotIsAwake(command.Channel, out bool didIWakeup, command.ChatMessage.IsBroadcaster))
            {
                sleeping = true;
            }

            if (_bans.Any(x => x.Username == command.ChatMessage.DisplayName.ToLower()) || sleeping)
                domainResult = DomainResult.Ignored;

            bool commandAuthValid = CheckCommandAuth(command.ChatMessage.IsBroadcaster, command.ChatMessage.IsModerator, commandOption);

            if (!commandAuthValid)
                domainResult = DomainResult.Ignored;

            BaseEvent @event = new PetEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{DisplayName}", command.ChatMessage.DisplayName);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void Sleep(SleepDomainCommand command)
        {
            bool ignore = true;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            if (CheckCommandAuth(command.ChatMessage.IsBroadcaster, command.ChatMessage.IsModerator, commandOption))
            {
                _lastSleepTime = DateTime.UtcNow;
                _wakeupTime = DateTime.MaxValue;
                _wakeLock = true;

                ignore = false;
            }

            var domainResult = DomainResult.Success;

            if (_bans.Any(x => x.Username == command.ChatMessage.DisplayName.ToLower()))
                ignore = true;

            if (!CheckIfBotIsAwake(command.Channel, out bool didIWakeup, command.ChatMessage.IsBroadcaster))
            {
                ignore = true;
            }

            if (ignore)
                domainResult = DomainResult.Ignored;

            BaseEvent @event = new SleepEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{Channel}", command.Channel);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        public void Wake(WakeDomainCommand command)
        {
            bool ignore = true;

            var commandOption = command.BotOption.BotCommands.Single(x => x.Command == command.CommandName);

            if (CheckCommandAuth(command.ChatMessage.IsBroadcaster, command.ChatMessage.IsModerator, commandOption))
            {
                _lastSleepTime = null;
                _wakeupTime = DateTime.UtcNow;
                _wakeLock = false;

                ignore = false;
            }

            var domainResult = DomainResult.Success;

            if (_bans.Any(x => x.Username == command.ChatMessage.DisplayName.ToLower()))
                ignore = true;

            if (CheckIfBotIsAwake(command.Channel, out bool didIWakeup, command.ChatMessage.IsBroadcaster))
            {
                ignore = true;
            }

            if (ignore)
                domainResult = DomainResult.Ignored;

            BaseEvent @event = new WakeEvent { Channel = command.Channel, TwitchClient = command.TwitchClient, DomainResult = domainResult };

            var eventName = @event.GetType().Name;

            var botEvent = commandOption.BotEvents.Single(x => x.Event == eventName);

            @event.Message = botEvent.GetResponseMessage(domainResult).Replace("{Channel}", command.Channel);

            if (@event.DomainResult == DomainResult.Success || @event.DomainResult == DomainResult.NotMommy)
                AddDomainEvent(@event);
        }

        #region Private

        private static bool CheckCommandAuth(bool isBrodCaster, bool IsModerator, BotCommand botCommand)
        {
            if (isBrodCaster)
                return true;
            else if (IsModerator && botCommand.AllowMods)
                return true;
            else if (botCommand.AllowEveryone)
                return true;
            else
                return false;
        }

        private bool CheckIfBotIsAwake(string channel, out bool didIWakeup, bool isBrodCaster = false)
        {
            didIWakeup = false;

            if (_lastSleepTime.HasValue)
            {
                if (_wakeupTime <= DateTime.UtcNow)
                {
                    _lastSleepTime = null;
                    _wakeupTime = DateTime.UtcNow;

                    return true;
                }
                else if (isBrodCaster)
                {
                    _lastSleepTime = null;
                    _wakeupTime = DateTime.UtcNow;

                    didIWakeup = true;

                    return true;
                }
            }
            else
                return true;

            return false;
        }

        private string GetRandomChatterName(string[] chatters, string[] knownBots)
        {
            if (chatters.Length == 0)
                return string.Empty;

            getRandomChatterNameIteration++;

            if (getRandomChatterNameIteration > 10)
                return string.Empty;

            string newChatterDisplayName = chatters[new Random().Next(0, chatters.Length - 1)];

            if (knownBots.Contains(newChatterDisplayName))
            {
                newChatterDisplayName = GetRandomChatterName(chatters, knownBots);
            }
            else if (!string.IsNullOrEmpty(_lastPokedChatterDisplayName) && !string.IsNullOrEmpty(newChatterDisplayName) && _lastPokedChatterDisplayName == newChatterDisplayName)
            {
                newChatterDisplayName = GetRandomChatterName(chatters, knownBots);
            }

            _lastPokedChatterDisplayName = newChatterDisplayName;
            return newChatterDisplayName;
        }
        #endregion
    }
}

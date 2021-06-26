using DoberDogBot.Application.Models;
using MediatR;
using TwitchLib.Client;

namespace DoberDogBot.Application.Commands
{
    public class BaseCommand : IRequest
    {
        public BotOption BotOption { get; set; }
        public TwitchClient TwitchClient { get; set; }
        public string Channel { get; set; }
        public string CommandName { get; protected set; }
        public int BotId { get; set; }
        public string SessionId { get; set; }
    }
}

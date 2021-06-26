using DoberDogBot.Application.Models;
using MediatR;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using TwitchLib.Client;
using TwitchLib.Client.Models;

namespace DoberDogBot.Application.Commands
{
    public interface IMessageService
    {
        Task ProcessMessage(ChatMessage ChatMessage, TwitchClient client, int botId, string sessionId);
    }

    public class MessageService : IMessageService
    {
        private readonly IMediator _mediator;
        private readonly BotOption _botOptions;

        public MessageService(IMediator mediator, IOptions<BotOption> botOptions)
        {
            _mediator = mediator;
            _botOptions = botOptions.Value;
        }

        public async Task ProcessMessage(ChatMessage chatMessage, TwitchClient client, int botId, string sessionId)
        {
            var command = GetCommand(chatMessage.Message);

            if (command.Equals(CommandsEnum.Attac.Name))
            {
                await _mediator.Send(new AttacCommand { Channel = chatMessage.Channel, ChatMessage = chatMessage, TwitchClient = client, BotOption = _botOptions, BotId = botId, SessionId = sessionId });
            }
            else if (command.Equals(CommandsEnum.Protec.Name))
            {
                await _mediator.Send(new ProtecCommand { Channel = chatMessage.Channel, ChatMessage = chatMessage, TwitchClient = client, BotOption = _botOptions, BotId = botId, SessionId = sessionId });
            }
            else if (command.Equals(CommandsEnum.Sleep.Name))
            {
                await _mediator.Send(new SleepCommand { Channel = chatMessage.Channel, ChatMessage = chatMessage, TwitchClient = client, BotOption = _botOptions, BotId = botId, SessionId = sessionId });
            }
            else if (command.Equals(CommandsEnum.Wake.Name))
            {
                await _mediator.Send(new WakeCommand { Channel = chatMessage.Channel, ChatMessage = chatMessage, TwitchClient = client, BotOption = _botOptions, BotId = botId, SessionId = sessionId });
            }
            else if (command.Equals(CommandsEnum.Pet.Name))
            {
                await _mediator.Send(new PetCommand { Channel = chatMessage.Channel, ChatMessage = chatMessage, TwitchClient = client, BotOption = _botOptions, BotId = botId, SessionId = sessionId });
            }
        }

        private static string GetCommand(string message)
        {
            message = message.Trim();

            string command = string.Empty;

            int strIndex = message.IndexOf(" ");

            if (message.StartsWith("!") && strIndex > 0)
            {
                command = message.Substring(0, strIndex);
            }
            else if (message.StartsWith("!") && strIndex == -1)
            {
                command = message;
            }
            return command;
        }
    }
}

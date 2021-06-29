using DoberDogBot.Application.Models;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class StreamEndCommand : BaseCommand
    {
        public string StreamEnded { get; set; }

        public StreamEndCommand()
        {
            CommandName = CommandsEnum.StreamEnd.Name;
        }
    }

    public class StreamEndCommandHandler : IRequestHandler<StreamEndCommand>
    {
        private readonly IStreamerRepository _streamerRepository;

        public StreamEndCommandHandler(IStreamerRepository streamerRepository)
        {
            _streamerRepository = streamerRepository;
        }

        public async Task<Unit> Handle(StreamEndCommand request, CancellationToken cancellationToken)
        {
            var streamer = await _streamerRepository.GetAsync(request.BotId.ToString());

            if (streamer != null)
            {
                streamer.SetSessionEndDate(request.SessionId, request.StreamEnded);

                await _streamerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
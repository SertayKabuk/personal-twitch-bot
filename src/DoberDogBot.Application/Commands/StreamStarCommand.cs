using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.StreamerAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class StreamStarCommand : BaseCommand<Unit>
    {
        public int PlayDelay { get; set; }
        public string StreamStartDate { get; set; }

        public StreamStarCommand()
        {
            CommandName = CommandsEnum.StreamStart.Name;
        }
    }

    public class StreamStarCommandProfile : Profile
    {
        public StreamStarCommandProfile()
        {
            CreateMap<StreamStarCommand, StreamStartDomainCommand>();
        }
    }

    public class StreamStartedCommandHandler : IRequestHandler<StreamStarCommand>
    {
        private readonly IMapper _mapper;
        private readonly IStreamerRepository _streamerRepository;

        public StreamStartedCommandHandler(IMapper mapper, IStreamerRepository streamerRepository)
        {
            _mapper = mapper;
            _streamerRepository = streamerRepository;
        }

        public async Task<Unit> Handle(StreamStarCommand request, CancellationToken cancellationToken)
        {
            var streamer = await _streamerRepository.GetAsync(request.BotId.ToString());

            if (streamer == null)
            {
                streamer = new Streamer(request.BotId.ToString(), request.Channel);
                _streamerRepository.Add(streamer);
            }

            streamer.AddSession(_mapper.Map<StreamStartDomainCommand>(request));

            await _streamerRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

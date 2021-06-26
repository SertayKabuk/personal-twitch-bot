using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace DoberDogBot.Application.Commands
{
    public class WakeCommand : BaseCommand
    {
        public ChatMessage ChatMessage { get; set; }

        public WakeCommand()
        {
            CommandName = CommandsEnum.Wake.Name;
        }
    }

    public class WakeCommandProfile : Profile
    {
        public WakeCommandProfile()
        {
            CreateMap<WakeCommand, WakeDomainCommand>();
        }
    }

    public class WakeCommandHandler : IRequestHandler<WakeCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public WakeCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(WakeCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.Wake(_mapper.Map<WakeDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

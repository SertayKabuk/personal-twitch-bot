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
    public class SleepCommand : BaseCommand<Unit>
    {
        public ChatMessage ChatMessage { get; set; }

        public SleepCommand()
        {
            CommandName = CommandsEnum.Sleep.Name;
        }
    }

    public class SleepCommandProfile : Profile
    {
        public SleepCommandProfile()
        {
            CreateMap<SleepCommand, SleepDomainCommand>();
        }
    }

    public class SleepCommandHandler : IRequestHandler<SleepCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public SleepCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(SleepCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.Sleep(_mapper.Map<SleepDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

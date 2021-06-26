using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class PokeChatterCommand : BaseCommand
    {
        public PokeChatterCommand()
        {
            CommandName = CommandsEnum.PokeChatter.Name;
        }

        public string[] Chatters { get; set; }
    }

    public class PokeChatterCommandProfile : Profile
    {
        public PokeChatterCommandProfile()
        {
            CreateMap<PokeChatterCommand, PokeChatterDomainCommand>();
        }
    }

    public class PokeChatterCommandHandler : IRequestHandler<PokeChatterCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public PokeChatterCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(PokeChatterCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.PokeChatter(_mapper.Map<PokeChatterDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

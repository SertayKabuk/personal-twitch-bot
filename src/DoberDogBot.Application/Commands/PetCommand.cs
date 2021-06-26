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
    public class PetCommand : BaseCommand
    {
        public ChatMessage ChatMessage { get; set; }

        public PetCommand()
        {
            CommandName = CommandsEnum.Pet.Name;
        }
    }

    public class PetCommandProfile : Profile
    {
        public PetCommandProfile()
        {
            CreateMap<PetCommand, PetDomainCommand>();
        }
    }

    public class PetCommandHandler : IRequestHandler<PetCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public PetCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(PetCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.Pet(_mapper.Map<PetDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

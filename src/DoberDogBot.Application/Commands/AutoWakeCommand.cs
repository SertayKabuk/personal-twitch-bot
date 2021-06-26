using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class AutoWakeCommand : BaseCommand
    {
        public AutoWakeCommand()
        {
            CommandName = CommandsEnum.AutoWake.Name;
        }
    }

    public class AutoWakeCommandProfile : Profile
    {
        public AutoWakeCommandProfile()
        {
            CreateMap<AutoWakeCommand, AutoWakeDomainCommand>();
        }
    }

    public class AutoWakeCommandHandler : IRequestHandler<AutoWakeCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public AutoWakeCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(AutoWakeCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.AutoWake(_mapper.Map<AutoWakeDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using FluentValidation;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib.Client.Models;

namespace DoberDogBot.Application.Commands
{
    public class ProtecCommand : BaseCommand<Unit>
    {
        public ChatMessage ChatMessage { get; set; }

        public ProtecCommand()
        {
            CommandName = CommandsEnum.Protec.Name;
        }
    }

    public class ProtecCommandProfile : Profile
    {
        public ProtecCommandProfile()
        {
            CreateMap<ProtecCommand, ProtecDomainCommand>();
        }
    }

    public class ProtecCommandValidator : AbstractValidator<ProtecCommand>
    {
        public ProtecCommandValidator()
        {
        }
    }

    public class ProtecCommandHandler : IRequestHandler<ProtecCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public ProtecCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(ProtecCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.Protec(_mapper.Map<ProtecDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

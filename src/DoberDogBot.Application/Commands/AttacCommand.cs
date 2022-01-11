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
    public class AttacCommand : BaseCommand<Unit>
    {
        public ChatMessage ChatMessage { get; set; }

        public AttacCommand()
        {
            CommandName = CommandsEnum.Attac.Name;
        }
    }

    public class AttacCommandProfile : Profile
    {
        public AttacCommandProfile()
        {
            CreateMap<AttacCommand, AttacDomainCommand>();
        }
    }

    public class AttackCommandValidator : AbstractValidator<AttacCommand>
    {
        public AttackCommandValidator()
        {
        }
    }

    public class AttackCommandHandler : IRequestHandler<AttacCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public AttackCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(AttacCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.Attac(_mapper.Map<AttacDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class BotOnJoinedCommand : BaseCommand
    {
        public BotOnJoinedCommand()
        {
            CommandName = CommandsEnum.BotOnJoined.Name;
        }
    }

    public class BotOnJoinedCommandProfile : Profile
    {
        public BotOnJoinedCommandProfile()
        {
            CreateMap<BotOnJoinedCommand, BotOnJoinedDomainCommand>();
        }
    }

    public class BotOnJoinedHandler : IRequestHandler<BotOnJoinedCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public BotOnJoinedHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(BotOnJoinedCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot == null)
            {
                bot = new Bot(request.BotId, null, DateTime.Now, false, string.Empty);
                _botRepository.Add(bot);
            }

            bot.BotOnJoined(_mapper.Map<BotOnJoinedDomainCommand>(request));


            await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

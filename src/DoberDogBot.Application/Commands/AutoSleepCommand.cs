﻿using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class AutoSleepCommand : BaseCommand
    {
        public AutoSleepCommand()
        {
            CommandName = CommandsEnum.AutoSleep.Name;
        }

        public int SleepDurationInMinutes { get; set; }
    }

    public class AutoSleepCommandProfile : Profile
    {
        public AutoSleepCommandProfile()
        {
            CreateMap<AutoSleepCommand, AutoSleepDomainCommand>();
        }
    }

    public class AutoSleepCommandHandler : IRequestHandler<AutoSleepCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public AutoSleepCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(AutoSleepCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.AutoSleep(_mapper.Map<AutoSleepDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

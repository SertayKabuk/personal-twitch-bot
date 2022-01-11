using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class TipDonationCommand : BaseCommand<Unit>
    {
        public string DisplayName { get; set; }
        public string Amount { get; set; }

        public TipDonationCommand()
        {
            CommandName = CommandsEnum.Tip.Name;
        }
    }

    public class TipDonationCommandProfile : Profile
    {
        public TipDonationCommandProfile()
        {
            CreateMap<TipDonationCommand, TipDonationDomainCommand>();
        }
    }

    public class TipDonationReceivedCommandHandler : IRequestHandler<TipDonationCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public TipDonationReceivedCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(TipDonationCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.TipDonation(_mapper.Map<TipDonationDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}
using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class BitDonationCommand : BaseCommand
    {
        public string DisplayName { get; set; }
        public string Bits { get; set; }

        public BitDonationCommand()
        {
            CommandName = CommandsEnum.Bit.Name;
        }
    }

    public class BitDonationCommandProfile : Profile
    {
        public BitDonationCommandProfile()
        {
            CreateMap<BitDonationCommand, BitDonationDomainCommand>();
        }
    }

    public class BitDonationReceivedCommandHandler : IRequestHandler<BitDonationCommand>
    {
        private readonly IMapper _mapper;
        private readonly IBotRepository _botRepository;

        public BitDonationReceivedCommandHandler(IMapper mapper, IBotRepository botRepository)
        {
            _mapper = mapper;
            _botRepository = botRepository;
        }

        public async Task<Unit> Handle(BitDonationCommand request, CancellationToken cancellationToken)
        {
            var bot = await _botRepository.GetAsync(request.BotId);

            if (bot != null)
            {
                bot.BitDonation(_mapper.Map<BitDonationDomainCommand>(request));

                await _botRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);
            }

            return Unit.Value;
        }
    }
}

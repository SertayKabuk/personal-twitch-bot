using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.BitAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class BitDonationCommand : BaseCommand<Unit>
    {
        public string Username { get; set; }
        public string ChannelName { get; set; }
        public string UserId { get; set; }
        public string ChannelId { get; set; }
        public string Time { get; set; }
        public string ChatMessage { get; set; }
        public int BitsUsed { get; set; }
        public int TotalBitsUsed { get; set; }
        public string Context { get; set; }
        public bool IsDonation { get; set; }

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
        private readonly IBitRepository _bitRepository;

        public BitDonationReceivedCommandHandler(IMapper mapper, IBitRepository bitRepository)
        {
            _mapper = mapper;
            _bitRepository = bitRepository;
        }

        public async Task<Unit> Handle(BitDonationCommand request, CancellationToken cancellationToken)
        {
            var bit = new Bit(request.Username,
                request.Channel,
                request.UserId,
                request.ChannelId,
                request.Time,
                request.ChatMessage,
                request.BitsUsed,
                request.TotalBitsUsed,
                request.Context,
                request.IsDonation,
                request.SessionId);

            _bitRepository.Add(bit);

            bit.BitReceived(_mapper.Map<BitDonationDomainCommand>(request));

            await _bitRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

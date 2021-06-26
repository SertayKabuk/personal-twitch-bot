using AutoMapper;
using DoberDogBot.Application.Models;
using DoberDogBot.Domain.AggregatesModel.SubscriberAggregate;
using DoberDogBot.Domain.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Commands
{
    public class SubscriberCommand : BaseCommand
    {
        public string DisplayName { get; set; }
        public string ChannelId { get; set; }
        public string SubMessage { get; set; }
        public string Context { get; set; }
        public int? StreakMonths { get; set; }
        public int? CumulativeMonths { get; set; }
        public int? Months { get; set; }
        public string SubscriptionPlanName { get; set; }
        public string SubscriptionPlan { get; set; }
        public bool? IsGift { get; set; }
        public DateTime Time { get; set; }
        public string UserId { get; set; }
        public string ChannelName { get; set; }
        public string RecipientDisplayName { get; set; }
        public string RecipientName { get; set; }
        public string Username { get; set; }
        public string RecipientId { get; set; }
        public int? MultiMonthDuration { get; set; }

        public SubscriberCommand()
        {
            CommandName = CommandsEnum.Subscriber.Name;
        }
    }

    public class SubscriberDonationCommandProfile : Profile
    {
        public SubscriberDonationCommandProfile()
        {
            CreateMap<SubscriberCommand, SubscriberDomainCommand>();
        }
    }

    public class SubscriberCommandHandler : IRequestHandler<SubscriberCommand>
    {
        private readonly IMapper _mapper;
        private readonly ISubscriberRepository _subscriberRepository;

        public SubscriberCommandHandler(IMapper mapper, ISubscriberRepository subscriberRepository)
        {
            _mapper = mapper;
            _subscriberRepository = subscriberRepository;
        }

        public async Task<Unit> Handle(SubscriberCommand request, CancellationToken cancellationToken)
        {
            var subscriber = new Subscriber(request.ChannelId,
                                            request.SubMessage,
                                            request.Context,
                                            request.StreakMonths,
                                            request.CumulativeMonths,
                                            request.Months,
                                            request.SubscriptionPlanName,
                                            request.SubscriptionPlan,
                                            request.IsGift,
                                            request.Time,
                                            request.UserId,
                                            request.ChannelName,
                                            request.RecipientDisplayName,
                                            request.RecipientName,
                                            request.DisplayName,
                                            request.Username,
                                            request.RecipientId,
                                            request.MultiMonthDuration,
                                            false,
                                            request.SessionId);

            _subscriberRepository.Add(subscriber);

            subscriber.SubscriberReceived(_mapper.Map<SubscriberDomainCommand>(request));

            await _subscriberRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}

using DoberDogBot.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DoberDogBot.Application.DomainEventHandlers
{
    public abstract class BaseEventHandler<T> : INotificationHandler<T> where T : BaseEvent
    {
        private readonly ILogger<BaseEventHandler<T>> logger;

        public BaseEventHandler(ILogger<BaseEventHandler<T>> logger)
        {
            this.logger = logger;
        }

        public virtual Task Handle(T notification, CancellationToken cancellationToken)
        {
            try
            {

                if (string.IsNullOrEmpty(notification.Channel) || string.IsNullOrEmpty(notification.Message))
                {
                    logger.LogWarning("Empty response:" + notification.GetType().Name);
                    return Task.CompletedTask;
                }

                if (notification.TwitchClient != null)
                    notification.TwitchClient.SendMessage(notification.Channel, notification.Message);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }

            return Task.CompletedTask;
        }
    }
}
using DoberDogBot.Domain.SeedWork;
using System.Threading.Tasks;

namespace DoberDogBot.Domain.AggregatesModel.SubscriberAggregate
{
    public interface ISubscriberRepository : IRepository<Subscriber>
    {
        Subscriber Add(Subscriber subscriber);
        void Update(Subscriber subscriber);
        Task<Subscriber> GetAsync(int id);
    }
}
using DoberDogBot.Domain.SeedWork;
using System.Threading.Tasks;

namespace DoberDogBot.Domain.AggregatesModel.BotAggregate
{
    public interface IBotRepository : IRepository<Bot>
    {
        Bot Add(Bot bot);
        void Update(Bot bot);
        Task<Bot> GetAsync(int botId);
    }
}
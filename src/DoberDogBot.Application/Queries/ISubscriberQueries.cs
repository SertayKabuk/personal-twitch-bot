using System.Threading.Tasks;

namespace DoberDogBot.Application.Queries
{
    public interface ISubscriberQueries
    {
        Task<int> GetDailySubCount(string channelId, string sessionId);
        Task<int> GetActiveSessionSubCount(string channelId);
    }
}

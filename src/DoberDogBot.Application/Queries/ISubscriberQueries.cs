using System.Threading.Tasks;

namespace DoberDogBot.Application.Queries
{
    public interface ISubscriberQueries
    {
        Task<int> GetDailySubCount(string channelId, string sessionId);
        Task<int> GetLastSessionSubCount(string channelId);
    }
}

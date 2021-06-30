using System.Threading.Tasks;

namespace DoberDogBot.Application.Queries
{
    public interface ISubscriberQueries
    {
        Task<int> GetSessionSubCount(string channelId, string sessionId);
        Task<int> GetActiveSessionSubCount(string channelId);
    }
}

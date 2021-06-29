using System.Threading.Tasks;

namespace DoberDogBot.Application.Queries
{
    public interface IStreamerQueries
    {
        Task<string> GetLastActiveSession(string channelId);
    }
}

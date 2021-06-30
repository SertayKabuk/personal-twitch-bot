using DoberDogBot.Infrastructure.AppDb;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Queries
{
    public class SubscriberQueries : ISubscriberQueries
    {
        private readonly AppDbContext _dbContext;

        public SubscriberQueries(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetActiveSessionSubCount(string channelId)
        {
            int subCount = 0;

            var streamer = await _dbContext.Streamers.SingleOrDefaultAsync(x => EF.Property<string>(x, "_channelId") == channelId);

            if (streamer != null)
            {
                await _dbContext.Entry(streamer).Collection(i => i.StreamerSessions).LoadAsync();

                var lastSessionId = streamer.StreamerSessions.LastOrDefault(x => x.StreamEndDate == null)?.SessionId;

                if (lastSessionId != null)
                {
                    subCount = await _dbContext.Subscribers.Where(x => x.ChannelId == channelId && x.SessionId == lastSessionId).CountAsync();
                }
            }
            return subCount;
        }

        public async Task<int> GetSessionSubCount(string channelId, string sessionId)
        {
            int subCount = await _dbContext.Subscribers.Where(x => x.ChannelId == channelId && x.SessionId == sessionId).CountAsync();

            return subCount;
        }
    }
}

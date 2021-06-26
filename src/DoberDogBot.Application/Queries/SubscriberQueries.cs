using DoberDogBot.Infrastructure.AppDb;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Queries
{
    public class SubscriberQueries : ISubscriberQueries
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public SubscriberQueries(IDbContextFactory<AppDbContext> dbFactor)
        {
            _dbFactory = dbFactor;
        }

        public async Task<int> GetLastSessionSubCount(string channelId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            var lastSessionId = await dbContext.Subscribers.OrderByDescending(x => x.Id).Where(x => x.ChannelId == channelId).Select(x => x.SessionId).FirstOrDefaultAsync();

            int subCount = await dbContext.Subscribers.Where(x => x.ChannelId == channelId && x.SessionId == lastSessionId).CountAsync();

            return subCount;
        }

        public async Task<int> GetDailySubCount(string channelId, string sessionId)
        {
            using var dbContext = _dbFactory.CreateDbContext();

            int subCount = await dbContext.Subscribers.Where(x => x.ChannelId == channelId && x.SessionId == sessionId).CountAsync();

            return subCount;
        }
    }
}

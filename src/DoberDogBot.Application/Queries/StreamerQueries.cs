using DoberDogBot.Infrastructure.AppDb;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace DoberDogBot.Application.Queries
{
    public class StreamerQueries : IStreamerQueries
    {
        private readonly AppDbContext _dbContext;

        public StreamerQueries(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<string> GetLastActiveSession(string channelId)
        {
            var streamer = await _dbContext.Streamers.SingleOrDefaultAsync(x => EF.Property<string>(x, "_channelId") == channelId);

            if (streamer != null)
            {
                await _dbContext.Entry(streamer).Collection(i => i.StreamerSessions).LoadAsync();

                var lastSessionId = streamer.StreamerSessions.LastOrDefault(x => x.StreamEndDate == null)?.SessionId;

                return lastSessionId;
            }

            return string.Empty;
        }
    }
}

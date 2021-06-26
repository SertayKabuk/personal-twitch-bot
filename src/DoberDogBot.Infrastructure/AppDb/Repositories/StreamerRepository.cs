using DoberDogBot.Domain.AggregatesModel.StreamerAggregate;
using DoberDogBot.Domain.SeedWork;
using DoberDogBot.Infrastructure.AppDb;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoberDogBot.Infrastructure.BotDb.Repositories
{
    public sealed class StreamerRepository : IStreamerRepository
    {
        private readonly AppDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public StreamerRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Streamer Add(Streamer streamer)
        {
            return _context.Streamers.Add(streamer).Entity;
        }

        public async Task<Streamer> GetAsync(int id)
        {
            var streamer = await _context.Streamers.SingleOrDefaultAsync(x => x.Id == id);

            if (streamer == null)
            {
                streamer = _context
                            .Streamers
                            .Local
                            .FirstOrDefault(o => o.Id == id);
            }


            return streamer;
        }

        public void Update(Streamer streamer)
        {
            _context.Entry(streamer).State = EntityState.Modified;
        }

    }
}
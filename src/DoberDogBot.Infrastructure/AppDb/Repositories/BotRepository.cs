using DoberDogBot.Domain.AggregatesModel.BotAggregate;
using DoberDogBot.Domain.SeedWork;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoberDogBot.Infrastructure.AppDb.Repositories
{
    public sealed class BotRepository : IBotRepository
    {
        private readonly AppDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public BotRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Bot Add(Bot bot)
        {
            return _context.Bots.Add(bot).Entity;
        }

        public async Task<Bot> GetAsync(int botId)
        {
           var bot =  await _context.Bots.SingleOrDefaultAsync(x => x.Id == botId);

            if (bot == null)
            {
                bot = _context
                            .Bots
                            .Local
                            .FirstOrDefault(o => o.Id == botId);
            }

            if (bot != null)
            {
                await _context.Entry(bot)
                    .Collection(i => i.Bans).LoadAsync();
            }

            return bot;
        }

        public void Update(Bot bot)
        {
            _context.Entry(bot).State = EntityState.Modified;
        }
    }
}
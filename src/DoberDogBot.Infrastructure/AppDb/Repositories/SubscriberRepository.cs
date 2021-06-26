using DoberDogBot.Domain.AggregatesModel.SubscriberAggregate;
using DoberDogBot.Domain.SeedWork;
using DoberDogBot.Infrastructure.AppDb;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoberDogBot.Infrastructure.BotDb.Repositories
{
    public sealed class SubscriberRepository : ISubscriberRepository
    {
        private readonly AppDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public SubscriberRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Subscriber Add(Subscriber subscriber)
        {
            return _context.Subscribers.Add(subscriber).Entity;
        }

        public async Task<Subscriber> GetAsync(int id)
        {
            var subscriber = await _context.Subscribers.SingleOrDefaultAsync(x => x.Id == id);

            if (subscriber == null)
            {
                subscriber = _context
                            .Subscribers
                            .Local
                            .FirstOrDefault(o => o.Id == id);
            }


            return subscriber;
        }

        public void Update(Subscriber subscriber)
        {
            _context.Entry(subscriber).State = EntityState.Modified;
        }
    }
}
using DoberDogBot.Domain.AggregatesModel.BitAggregate;
using DoberDogBot.Domain.SeedWork;
using DoberDogBot.Infrastructure.AppDb;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DoberDogBot.Infrastructure.BotDb.Repositories
{
    public sealed class BitRepository : IBitRepository
    {
        private readonly AppDbContext _context;

        public IUnitOfWork UnitOfWork => _context;

        public BitRepository(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Bit Add(Bit bit)
        {
            return _context.Bits.Add(bit).Entity;
        }

        public async Task<Bit> GetAsync(int id)
        {
            var bit = await _context.Bits.SingleOrDefaultAsync(x => x.Id == id);

            if (bit == null)
            {
                bit = _context
                            .Bits
                            .Local
                            .FirstOrDefault(o => o.Id == id);
            }


            return bit;
        }

        public void Update(Bit bit)
        {
            _context.Entry(bit).State = EntityState.Modified;
        }
    }
}
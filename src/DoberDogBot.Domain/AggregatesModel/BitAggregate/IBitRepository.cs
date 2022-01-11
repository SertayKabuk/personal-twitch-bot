using DoberDogBot.Domain.SeedWork;
using System.Threading.Tasks;

namespace DoberDogBot.Domain.AggregatesModel.BitAggregate
{
    public interface IBitRepository : IRepository<Bit>
    {
        Bit Add(Bit bit);
        void Update(Bit bit);
        Task<Bit> GetAsync(int id);
    }
}
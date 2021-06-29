using DoberDogBot.Domain.AggregatesModel.StreamerAggregate;
using DoberDogBot.Domain.SeedWork;
using System.Threading.Tasks;

public interface IStreamerRepository : IRepository<Streamer>
{
    Streamer Add(Streamer streamer);
    void Update(Streamer streamer);
    Task<Streamer> GetAsync(int id);
    Task<Streamer> GetAsync(string channelId);
}
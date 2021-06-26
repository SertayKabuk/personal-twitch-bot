using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace DoberDogBot.UI.Hubs
{
    public class TwitchSubHub : Hub
    {
        public string GetConnectionId() => Context.ConnectionId;

        public async Task AddToGroup(string groupName) => await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        public async Task RemoveFromGroup(string groupName) => await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }
}

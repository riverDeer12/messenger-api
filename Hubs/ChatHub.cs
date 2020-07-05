using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace MessengerAPI.Hubs
{
    public class ChatHub : Hub
    {
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}

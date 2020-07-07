using MessengerAPI.Data.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessengerAPI.Hubs
{
    public class MessageHub : Hub
    {
        public Task SendPublicMessage(Message message)
        {
            return Clients.All.SendAsync("receiveMessage", message);
        }

        public async Task SendMessageToChat(Message message, string chatname)
        {
            await Clients.Group(chatname).SendAsync("receiveMessage", message);
        }

        public string GetConnectionId() => Context.ConnectionId;
    }
}

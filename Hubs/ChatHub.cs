using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class ChatHub : Hub
    {
        public void Send(string chatName, string userName, string message)
        {
            Clients.All.newMessage(chatName, userName, message);
        }
    }
}
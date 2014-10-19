using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnection = new ConcurrentDictionary<string, string>();

        public override Task OnConnected()
        {
            UserJoinedChat();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserLeftChat();
            return base.OnDisconnected(stopCalled);
        }

        public Task Send(string message)
        {
            var userName = UserConnection[Context.ConnectionId];
            return Clients.Others.newMessage(userName, message);
        }

        private async void UserJoinedChat()
        {
            var userName = Context.User.Identity.Name;
            var userExists = UserConnection.Values.Any(x => x == userName);
            UserConnection[Context.ConnectionId] = userName;
            await Clients.Caller.UsersInChat(UserConnection.Values.Distinct());
            if (!userExists)
            {
                await Clients.Others.UserJoinedChat(userName);
            }
        }

        private async void UserLeftChat()
        {
            string userName;
            UserConnection.TryRemove(Context.ConnectionId, out userName);
            await Clients.Others.userLeftChat(userName);
        }
    }
}
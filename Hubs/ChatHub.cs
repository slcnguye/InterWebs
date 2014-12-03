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

        public void AddUser(string username)
        {
            var userExists = UserConnection.Values.Any(x => x == username);
            UserConnection[Context.ConnectionId] = username;
            if (!userExists)
            {
                Task.Run(() => Clients.Others.UserJoinedChat(username));
            }
        }

        private void UserJoinedChat()
        {
            var userName = Context.RequestCookies.ContainsKey("username")
                ? Context.RequestCookies["username"].Value
                : null;

            Task.Run(() => Clients.Caller.UsersInChat(UserConnection.Values.Distinct()));

            if (userName == null)
            {
                return;
            }

            var userExists = UserConnection.Values.Any(x => x == userName);
            UserConnection[Context.ConnectionId] = userName;
            if (!userExists)
            {
                Task.Run(() => Clients.Others.UserJoinedChat(userName));
            }
        }

        private void UserLeftChat()
        {
            string userName;
            UserConnection.TryRemove(Context.ConnectionId, out userName);
            Task.Run(() => Clients.Others.userLeftChat(userName));
        }

        public void Send(string message)
        {
            var userName = UserConnection[Context.ConnectionId];
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            Task.Run(() => Clients.Others.newMessage(userName, message));
        }
    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, List<string>> ChatUsers = new Dictionary<string, List<string>>();  

        public Task Send(string chatName, string message)
        {
            var userName = Context.User.Identity.Name;
            return Clients.Others.newMessage(chatName, userName, message);
        }

        public void JoinChat(string chatName)
        {
            var userName = Context.User.Identity.Name;
            if (!ChatUsers.ContainsKey(chatName))
            {
                ChatUsers[chatName] = new List<string>();
            }
            var chatUsers = ChatUsers[chatName];
            Clients.Caller.UsersInChat(chatName, chatUsers);
            chatUsers.Add(userName);
            Clients.Others.UserJoinedChat(chatName, userName);
        }

        public Task LeaveChat(string chatName)
        {
            var userName = Context.User.Identity.Name;
            ChatUsers[chatName].RemoveAll(x => x == userName);
            return Clients.Others.userLeftChat(chatName, userName);
        }
    }
}
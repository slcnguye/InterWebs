using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class ChatHub : Hub
    {
        private static readonly Dictionary<string, List<string>> ChatUsers = new Dictionary<string, List<string>>();  

        public void Send(string chatName, string userName, string message)
        {
            Clients.Others.newMessage(chatName, userName, message);
        }

        public void JoinChat(string chatName, string userName)
        {
            if (!ChatUsers.ContainsKey(chatName))
            {
                ChatUsers[chatName] = new List<string>();
            }
            var chatUsers = ChatUsers[chatName];
            Clients.Caller.UsersInChat(chatName, chatUsers);
            chatUsers.Add(userName);
            Clients.Others.UserJoinedChat(chatName, userName);
        }

        public void LeaveChat(string chatName, string userName)
        {
            Clients.Others.userLeftChat(chatName, userName);
            ChatUsers[chatName].RemoveAll(x => x == userName);
        }
    }
}
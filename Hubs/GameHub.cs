using System.Collections.Generic;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class GameHub : Hub
    {
        private static readonly Dictionary<string, List<string>> GameUsers = new Dictionary<string, List<string>>();  

        public void JoinGame(string gameName, string userName)
        {
            if (!GameUsers.ContainsKey(gameName))
            {
                GameUsers[gameName] = new List<string>();
            }
            var chatUsers = GameUsers[gameName];
            Clients.Caller.UsersInGame(gameName, chatUsers);
            chatUsers.Add(userName);
            Clients.Others.UserJoinedGame(gameName, userName);
        }

        public void LeaveGame(string gameName, string userName)
        {
            Clients.Others.userLeftGame(gameName, userName);
            GameUsers[gameName].RemoveAll(x => x == userName);
        }
    }
}
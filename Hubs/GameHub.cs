using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class GameHub : Hub
    {
        private static readonly Dictionary<string, List<string>> GameUsers = new Dictionary<string, List<string>>();  
        private static readonly Random Random = new Random();

        private static readonly List<int> Player1 = new List<int>();
        private static readonly List<int> Player2 = new List<int>();

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

        public void GetCardsForPlayer(string gameName, int player)
        {
            var playersHand = player == 1 ? Player1 : Player2;
            if (!playersHand.Any())
            {
                playersHand.Add(-1);    
                playersHand.Add(-1);    
            }

            Clients.Caller.PlayersHand(gameName, player, playersHand);
        }

        public void DrawCard(string gameName, int player, int cardIndex)
        {
            var newCard = Random.Next(0,52);
            var playersHand = player == 1 ? Player1 : Player2;
            playersHand[cardIndex] = newCard;
            Clients.All.DrawCard(gameName, player, cardIndex, newCard);
        }
    }
}
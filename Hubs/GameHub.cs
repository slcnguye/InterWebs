using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InterWebs.Models.Game;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class GameHub : Hub
    {
        private static readonly List<string> GameUsers;  
        private static readonly Deck Deck;
        private static readonly Player[] Players;

        static GameHub()
        {
            GameUsers = new List<string>();
            Deck = new Deck();
            Deck.Shuffle();
            var player1 = new Player {Id = 0};
            player1.Cards.Add(-1);
            player1.Cards.Add(-1);
            var player2 = new Player {Id = 1};
            player2.Cards.Add(-1);
            player2.Cards.Add(-1);
            Players = new[] {player1, player2};
        }

        public void JoinGame()
        {
            var userName = Context.User.Identity.Name;
            Clients.Caller.UsersInGame(GameUsers);
            GameUsers.Add(userName);
            Clients.Others.UserJoinedGame(userName);
        }

        public Task LeavePage()
        {
            var userName = Context.User.Identity.Name;
            GameUsers.RemoveAll(x => x == userName);
            return Clients.Others.UserLeftGame(userName);
        }

        public Player[] GetAllPlayers()
        {
            return Players;
        }

        public Task DrawCard(int cardIndex)
        {
            var playerName = Context.User.Identity.Name;
            var player = Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                return null;
            }

            var newCard = Deck.Draw();
            player.Cards[cardIndex] = newCard;
            return Clients.All.DrawCard(player.Id, cardIndex, newCard);
        }

        public Task JoinGameTable(int player)
        {
            var userName = Context.User.Identity.Name;
            Players[player].Name = userName;
            return Clients.Others.UserJoinedGameTable(userName, player);
        }

        public Task LeaveGameTable(int player)
        {
            Players[player].Name = "";
            return Clients.Others.UserLeftGameTable(player);
        }
    }
}
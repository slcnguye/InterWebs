using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using InterWebs.Models.Game;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class GameHub : Hub
    {
        private static readonly ConcurrentDictionary<string, string> UserConnection = new ConcurrentDictionary<string, string>();

        private static readonly Deck Deck;
        private static readonly Player[] Players;

        static GameHub()
        {
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

        public override Task OnConnected()
        {
            UserJoinedGame();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            UserLeftGame();
            return base.OnDisconnected(stopCalled);
        }

        private async void UserJoinedGame()
        {
            var userName = Context.User.Identity.Name;
            var userExists = UserConnection.Values.Any(x => x == userName);
            UserConnection[Context.ConnectionId] = userName;
            await Clients.Caller.UsersInGame(UserConnection.Values.Distinct());
            if (!userExists)
            {
                await Clients.Others.UserJoinedGame(userName);
            }
        }

        private async void UserLeftGame()
        {
            string userName;
            UserConnection.TryRemove(Context.ConnectionId, out userName);

            for (var i = 0; i < 2; ++i)
            {
                if (Players[i].Name != userName)
                {
                    continue;
                }

                Players[i].Name = "";
                await Clients.Others.UserLeftGameTable(i);
            }

            await Clients.Others.UserLeftGame(userName);
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

        public async void ShuffleDeck()
        {
            Deck.Shuffle();
            foreach (var player in Players)
            {
                await Clients.All.DrawCard(player.Id, 0, -1);
                await Clients.All.DrawCard(player.Id, 1, -1);
            }
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
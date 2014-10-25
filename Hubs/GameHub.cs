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

        private static readonly WarGame WarGame;
        private static int? p1CardPlayed;
        private static int? p2CardPlayed;

        static GameHub()
        {
            WarGame = new WarGame();
            WarGame.StartNewGame();
            p1CardPlayed = null;
            p2CardPlayed = null;
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
                if (WarGame.Players[i].Name != userName)
                {
                    continue;
                }

                WarGame.Players[i].Name = "";
                await Clients.Others.UserLeftGameTable(i);
            }

            await Clients.Others.UserLeftGame(userName);
        }

        public Player[] GetAllPlayers()
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            return player == null ? null : WarGame.Players;
        }

        public object GetAllPlayerNames()
        {
            return WarGame.Players.Select(x => new { x.Id, x.Name });
        }

        public async void PlayCard(int cardIndex)
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                return;
            }

            if (player.Id == 0)
            {
                p1CardPlayed = cardIndex;
            }
            if (player.Id == 1)
            {
                p2CardPlayed = cardIndex;
            }

            if (p1CardPlayed == null || p2CardPlayed == null)
            {
                return;
            }

            Player winner;
            WarGame.PlayRound(p1CardPlayed.Value, p2CardPlayed.Value, out winner);
            await Clients.All.RoundWinner(winner.Id);
            await Clients.Group("playing").DrawCard(0, p1CardPlayed.Value, WarGame.Players[0].Cards[p1CardPlayed.Value].Value);
            await Clients.Group("playing").DrawCard(1, p2CardPlayed.Value, WarGame.Players[1].Cards[p2CardPlayed.Value].Value);
            p1CardPlayed = null;
            p2CardPlayed = null;
        }

        public void UnplayCard(int cardIndex)
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                return;
            }

            if (player.Id == 0)
            {
                p1CardPlayed = null;
            }
            if (player.Id == 1)
            {
                p2CardPlayed = null;
            }
        }

        public async void ShuffleDeck()
        {
            WarGame.StartNewGame();
            foreach (var player in WarGame.Players)
            {
                await Clients.Group("playing").DrawCard(player.Id, 0, player.Cards[0].Value);
                await Clients.Group("playing").DrawCard(player.Id, 1, player.Cards[1].Value);
            }
        }

        public Task JoinGameTable(int player)
        {
            var userName = Context.User.Identity.Name;
            WarGame.Players[player].Name = userName;
            Groups.Add(Context.ConnectionId, "playing");
            return Clients.Others.UserJoinedGameTable(userName, player);
        }

        public Task LeaveGameTable(int player)
        {
            WarGame.Players[player].Name = "";
            Groups.Remove(Context.ConnectionId, "playing");
            return Clients.Others.UserLeftGameTable(player);
        }
    }
}
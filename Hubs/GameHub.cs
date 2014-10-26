using System.Collections.Concurrent;
using System.Collections.Generic;
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

        static GameHub()
        {
            WarGame = new WarGame();
            WarGame.StartNewGame();
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

        public IEnumerable<Player> GetAllPlayerNames()
        {
            return WarGame.Players.Select(x => new Player {Id = x.Id, Name = x.Name, PlayedCard = x.PlayedCard});
        }

        public async void PlayCard(int cardIndex)
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                return;
            }

            player.PlayedCard = cardIndex;
            await Clients.Others.CardPlayed(player.Id, cardIndex);

            if (WarGame.Players.Any(x => x.PlayedCard == null)) 
            {
                return;
            }

            var p1CardPlayed = WarGame.Players[0].PlayedCard.Value;
            var p2CardPlayed = WarGame.Players[1].PlayedCard.Value;

            Player winner;
            WarGame.PlayRound(out winner);
            

            await Clients.All.RoundWinner(winner.Id);
            await Clients.Group("playing").DrawCard(0, p1CardPlayed, WarGame.Players[0].Cards[p1CardPlayed].Value);
            await Clients.Group("playing").DrawCard(1, p2CardPlayed, WarGame.Players[1].Cards[p2CardPlayed].Value);
        }

        public async void UnplayCard(int cardIndex)
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                return;
            }

            player.PlayedCard = null;
            
            await Clients.Others.CardUnplayed(player.Id);
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
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

        public Player GetPlayer(int position)
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            return player;
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

            if (WarGame.Players.Any(x => x.PlayedCard == -1)) 
            {
                return;
            }

            var player1 = WarGame.Players[0];
            var player2 = WarGame.Players[1];
            var p1CardPlayed = player1.PlayedCard;
            var p2CardPlayed = player2.PlayedCard;

            Clients.All.ShowCard(player1.Id, p1CardPlayed, player1.Cards[p1CardPlayed].Value);
            Clients.All.ShowCard(player2.Id, p2CardPlayed, player2.Cards[p2CardPlayed].Value);
            
            Player winner;
            WarGame.PlayRound(out winner);
            if (winner == null)
            {
                return;
            }

            await Clients.All.RoundWinner(winner.Id);
        }

        public int GetCard(int cardIndex, int playerId)
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName && x.Id == playerId);
            if (player == null)
            {
                return -1;
            }

            return player.Cards[cardIndex].Value;
        }

        public async void UnplayCard(int cardIndex)
        {
            var playerName = Context.User.Identity.Name;
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null)
            {
                return;
            }

            player.PlayedCard = -1;
            
            await Clients.Others.CardUnplayed(player.Id);
        }

        public async void ShuffleDeck()
        {
            WarGame.StartNewGame();
            foreach (var player in WarGame.Players)
            {
                await Clients.Group(player.Name).DrawCard(player.Id, 0, player.Cards[0].Value);
                await Clients.Group(player.Name).DrawCard(player.Id, 1, player.Cards[1].Value);
            }
        }

        public Task JoinGameTable(int player)
        {
            var userName = Context.User.Identity.Name;
            Groups.Add(Context.ConnectionId, userName);
            WarGame.Players[player].Name = userName;
            return Clients.Others.UserJoinedGameTable(userName, player);
        }

        public Task LeaveGameTable(int player)
        {
            Groups.Remove(Context.ConnectionId, WarGame.Players[player].Name);
            WarGame.Players[player].Name = "";
            return Clients.Others.UserLeftGameTable(player);
        }
    }
}
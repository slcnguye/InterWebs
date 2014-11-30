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

        private void UserJoinedGame()
        {
            var userName = Context.RequestCookies.ContainsKey("username")
                ? Context.RequestCookies["username"].Value
                : null;
            
            Task.Run(() => Clients.Caller.UsersInGame(UserConnection.Values.Distinct()));

            if (userName == null)
            {
                return;
            }

            AddUser(userName);
        }

        private void UserLeftGame()
        {
            string userName;
            UserConnection.TryRemove(Context.ConnectionId, out userName);
            
            for (var i = 0; i < 2; ++i)
            {
                if (WarGame.Players[i].Name != userName)
                {
                    continue;
                }

                WarGame.Players[i].Name = string.Empty;
                var playerIndex = i;
                Task.Run(() => Clients.Others.UserLeftGameTable(playerIndex));
            }

            Task.Run(() => Clients.Others.UserLeftGame(userName));
        }

        public void AddUser(string username)
        {
            var userExists = UserConnection.Values.Any(x => x == username);
            UserConnection[Context.ConnectionId] = username;
            if (!userExists)
            {
                Task.Run(() => Clients.Others.UserJoinedGame(username));
            }
        }

        public Player GetPlayer(int position)
        {
            var playerName = GetUserName();
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            return player;
        }

        public IEnumerable<Player> GetAllPlayerNames()
        {
            return WarGame.Players.Select(x => new Player {Id = x.Id, Name = x.Name, PlayedCardIndex = x.PlayedCardIndex});
        }

        public void PlayCard(int cardIndex)
        {
            var playerName = GetUserName();
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null || playerName == string.Empty)
            {
                return;
            }

            player.PlayedCardIndex = cardIndex;
            Task.Run(() => Clients.Others.CardPlayed(player.Id, cardIndex));

            if (WarGame.Players.Any(x => x.PlayedCardIndex == -1)) 
            {
                return;
            }

            var player1 = WarGame.Players[0];
            var p1CardPlayedIndex = player1.PlayedCardIndex;
            var p1CardPlayedValue = player1.Cards[p1CardPlayedIndex].Value;
            
            var player2 = WarGame.Players[1];
            var p2CardPlayedIndex = player2.PlayedCardIndex;
            var p2CardPlayedValue = player2.Cards[p2CardPlayedIndex].Value;

            Task.Run(() => Clients.All.ShowCard(player1.Id, p1CardPlayedIndex, p1CardPlayedValue));
            Task.Run(() => Clients.All.ShowCard(player2.Id, p2CardPlayedIndex, p2CardPlayedValue));
            
            Player winner;
            WarGame.RoundOutCome outcome;
            WarGame.PlayRound(out winner, out outcome);

            switch (outcome)
            {
                case WarGame.RoundOutCome.Draw:
                    Task.Run(() => Clients.All.RoundOutcome("Draw"));
                    break;
                case WarGame.RoundOutCome.PlayerWonGame:
                    Task.Run(() => Clients.All.RoundOutcome(winner.Name + " won the game!"));
                    break;
                case WarGame.RoundOutCome.PlayerWonRound:
                    Task.Run(() => Clients.All.RoundOutcome(winner.Name + " won the round!"));
                    break;
            }
        }

        public bool IsBlankCard(int cardIndex, int playerId)
        {
            var player = WarGame.Players.FirstOrDefault(x => x.Id == playerId);
            if (player == null)
            {
                return true;
            }

            return player.Cards[cardIndex].Value == -2;
        }

        public int GetCard(int cardIndex, int playerId)
        {
            var playerName = GetUserName();
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName && x.Id == playerId);
            if (player == null)
            {
                return -1;
            }

            return player.Cards[cardIndex].Value;
        }

        public void UnplayCard(int cardIndex)
        {
            var playerName = GetUserName();
            var player = WarGame.Players.FirstOrDefault(x => x.Name == playerName);
            if (player == null || playerName == string.Empty)
            {
                return;
            }

            player.PlayedCardIndex = -1;
            
            Task.Run(() => Clients.Others.CardUnplayed(player.Id));
        }

        public void ShuffleDeck()
        {
            WarGame.StartNewGame();
            Task.Run(() => Clients.All.NewGame());
            foreach (var player in WarGame.Players)
            {
                var playerCopy = player;
                Task.Run(() => Clients.Group(playerCopy.Name).DrawCard(playerCopy.Id, 0, playerCopy.Cards[0].Value));
                Task.Run(() => Clients.Group(playerCopy.Name).DrawCard(playerCopy.Id, 1, playerCopy.Cards[1].Value));
            }
        }

        public void JoinGameTable(int player)
        {
            var userName = GetUserName();
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            Groups.Add(Context.ConnectionId, userName);
            WarGame.Players[player].Name = userName;
            Task.Run(() => Clients.Others.UserJoinedGameTable(userName, player));
        }

        public void LeaveGameTable(int player)
        {
            Groups.Remove(Context.ConnectionId, WarGame.Players[player].Name);
            WarGame.Players[player].Name = string.Empty;
            Task.Run(() => Clients.Others.UserLeftGameTable(player));
        }

        private string GetUserName()
        {
            return UserConnection[Context.ConnectionId];
        }
    }
}
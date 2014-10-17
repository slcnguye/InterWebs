using System.Collections.Generic;
using System.Linq;
using InterWebs.Models.Game;
using Microsoft.AspNet.SignalR;

namespace InterWebs.Hubs
{
    public class GameHub : Hub
    {
        private static readonly List<string> GameUsers = new List<string>();  
        
        private static readonly Deck Deck = new Deck();
        private static readonly Player Player1 = new Player { Id = 0 };
        private static readonly Player Player2 = new Player { Id = 1 };

        public void JoinGame(string userName)
        {
            Clients.Caller.UsersInGame(GameUsers);
            GameUsers.Add(userName);
            Clients.Others.UserJoinedGame(userName);
        }

        public void LeaveGame(string userName)
        {
            Clients.Others.UserLeftGame(userName);
            GameUsers.RemoveAll(x => x == userName);
        }

        public void GetCardsForPlayer(int player)
        {
            var playerInfo = GetPlayer(player);
            var playersHand = playerInfo.Cards;
            if (!playersHand.Any())
            {
                playersHand.Add(-1);    
                playersHand.Add(-1);
                Deck.Shuffle();
            }

            Clients.Caller.PlayersHand(playerInfo.Name, player, playersHand);
        }

        public void DrawCard(int player, int cardIndex)
        {
            var newCard = Deck.Draw();
            var playersHand = GetPlayer(player).Cards;
            playersHand[cardIndex] = newCard;
            Clients.All.DrawCard(player, cardIndex, newCard);
        }

        public void JoinGameTable(string userName, int player)
        {
            GetPlayer(player).Name = userName;
            Clients.Others.UserJoinedGameTable(userName, player);
        }

        public void LeaveGameTable(int player)
        {
            GetPlayer(player).Name = "";
            Clients.Others.UserLeftGameTable(player);
        }

        private Player GetPlayer(int id)
        {
            return id == Player1.Id ? Player1 : Player2;
        }
    }
}
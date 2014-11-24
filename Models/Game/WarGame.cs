using System;
using System.Linq;

namespace InterWebs.Models.Game
{
    public class WarGame
    {
        private static readonly Random Random = new Random();

        private Deck player1Deck;
        private Deck player2Deck;
        private readonly Player player1;
        private readonly Player player2;
        private const int CardsPerHand = 2;

        public enum RoundOutCome
        {
            PlayerWonGame,
            PlayerWonRound,
            Draw
        }

        public WarGame()
        {
            player1 = new Player {Id = 0};
            player2 = new Player {Id = 1};
            Players = new [] {player1, player2};
        }

        public Player[] Players { get; private set; }

        public void StartNewGame()
        {
            var cards = Enumerable.Range(0, 52).OrderBy(card => Random.Next()).ToList();
            player1Deck = new Deck(cards.GetRange(0, 26));
            player2Deck = new Deck(cards.GetRange(26, 26));

            player1.PlayedCardIndex = -1;
            player2.PlayedCardIndex = -1;
            player1.Cards.Clear();
            player2.Cards.Clear();
            for (var i = 0; i < CardsPerHand; i++)
            {
                player1.Cards.Add(player1Deck.DrawCard());
                player2.Cards.Add(player2Deck.DrawCard());
            }
        }

        public void PlayRound(out Player winner, out RoundOutCome outcome)
        {
            winner = null;
            var p1Card = player1.Cards[player1.PlayedCardIndex];
            var p2Card = player2.Cards[player2.PlayedCardIndex];


            var compareRank = p1Card.CompareTo(p2Card);
            if (compareRank == 0)
            {
                player1Deck.AddCard(p1Card, true);
                player2Deck.AddCard(p2Card, true);
                outcome = RoundOutCome.Draw;
            }
            else
            {
                var player1Wins = compareRank > 0;
                var winnersDeck = player1Wins ? player1Deck : player2Deck;
                winner = player1Wins ? player1 : player2;
                winnersDeck.AddCard(p1Card, true);
                winnersDeck.AddCard(p2Card, true);   
                outcome = RoundOutCome.PlayerWonRound;
            }

            // Draw new cards
            player1.Cards[player1.PlayedCardIndex] = player1Deck.DrawCard();
            player2.Cards[player2.PlayedCardIndex] = player2Deck.DrawCard();
            player1.PlayedCardIndex = -1;
            player2.PlayedCardIndex = -1;

            if (player1.Cards.All(x => x.Value < 0))
            {
                outcome = RoundOutCome.PlayerWonGame;
                winner = player2;
            }

            if (player2.Cards.All(x => x.Value < 0))
            {
                outcome = RoundOutCome.PlayerWonGame;
                winner = player1;
            }
        }
    }
}
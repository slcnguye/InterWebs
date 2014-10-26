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
            
            player1.Cards.Clear();
            player2.Cards.Clear();
            for (var i = 0; i < CardsPerHand; i++)
            {
                player1.Cards.Add(player1Deck.DrawCard());
                player2.Cards.Add(player2Deck.DrawCard());
            }
        }

        public void PlayRound(out Player winner)
        {
            winner = null;
            // Run out of cards
            if (player1.PlayedCard == null || player1.PlayedCard == -1)
            {
                //player1 Lost;
                return;
            }

            if (player2.PlayedCard == null || player2.PlayedCard == -1)
            {
                //player 2 Lost;
                return;
            }

            // Winner of war battle
            var p1Card = player1.Cards[player1.PlayedCard.Value];
            var p2Card = player2.Cards[player2.PlayedCard.Value];
            var winnersDeck = p1Card.CompareTo(p2Card) > 0 ? player1Deck : player2Deck;
            winner = p1Card.CompareTo(p2Card) > 0 ? player1 : player2;
            winnersDeck.AddCard(p1Card, true);
            winnersDeck.AddCard(p2Card, true);

            // Draw new cards
            player1.Cards[player1.PlayedCard.Value] = player1Deck.DrawCard();
            player2.Cards[player2.PlayedCard.Value] = player2Deck.DrawCard();
            player1.PlayedCard = null;
            player2.PlayedCard = null;
        }
    }
}
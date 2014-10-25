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

        public void PlayRound(int p1CardPlayed, int p2CardPlayed, out Player winner)
        {
            // Run out of cards
            if (p1CardPlayed == -1)
            {
                //player1 Lost;
            }

            if (p2CardPlayed == -1)
            {
                //player 2 Lost;
            }

            // Winner of war battle
            var p1Card = player1.Cards[p1CardPlayed];
            var p2Card = player2.Cards[p2CardPlayed];
            var winnersDeck = p1Card.CompareTo(p2Card) > 0 ? player1Deck : player2Deck;
            winner = p1Card.CompareTo(p2Card) > 0 ? player1 : player2;
            winnersDeck.AddCard(p1Card, true);
            winnersDeck.AddCard(p2Card, true);

            // Draw new cards
            player1.Cards[p1CardPlayed] = player1Deck.DrawCard();
            player2.Cards[p2CardPlayed] = player2Deck.DrawCard();
        }
    }
}
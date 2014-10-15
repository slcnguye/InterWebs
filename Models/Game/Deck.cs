using System;
using System.Linq;

namespace InterWebs.Models.Game
{
    public class Deck
    {
        private const int NumberOfCards = 52;
        private readonly Random random = new Random();
        private int deckIndex;
        private int[] deck = Enumerable.Range(0, NumberOfCards).ToArray();

        public void Shuffle()
        {
            deckIndex = NumberOfCards;
            deck = deck.OrderBy(a => random.Next()).ToArray();
        }

        public int Draw()
        {
            deckIndex--;
            return deckIndex < 0 ? -1 : deck[deckIndex];
        }
    }
}
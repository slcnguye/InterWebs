using System;
using System.Collections.Generic;
using System.Linq;

namespace InterWebs.Models.Game
{
    public class Deck
    {
        private static readonly Random Random = new Random();
        private static readonly Card BlankCard = new Card { Value = -2 };
        private List<Card> cards;

        public Deck()
        {
            cards = Enumerable.Range(0, 52).Select(x => new Card { Value = x }).ToList();
        }

        public Deck(IEnumerable<int> cards)
        {
            this.cards = cards.Select(x => new Card { Value = x }).ToList();
        }

        public void Shuffle()
        {
            cards = cards.OrderBy(a => Random.Next()).ToList();
        }

        public Card DrawCard()
        {
            if (!cards.Any())
            {
                return BlankCard;
            }

            var cardDrawn = cards.First();
            cards.RemoveAt(0);
            return cardDrawn;
        }

        public void AddCard(Card card, bool randomInsert = false)
        {
            if (randomInsert)
            {
                cards.Insert(Random.Next(cards.Count), card);
            }
            else
            {
                cards.Add(card);
            }
        }

        public void AddCards(IEnumerable<Card> cardsToAdd)
        {
            cards.AddRange(cardsToAdd);
        }
    }
}
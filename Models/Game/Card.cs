using System;
using System.Collections.Generic;
using Microsoft.Ajax.Utilities;

namespace InterWebs.Models.Game
{
    public class Card : IComparable<Card>
    {
        public static List<string> CardsSource;
        public static dynamic CardsValue;

        public int Value { get; set; }

        public int CompareTo(Card card)
        {
            if (card == null)
            {
                return 1;
            }

            int thisValue = CardsValue[CardsSource[Value].SubstringUpToFirst('_')] ?? -1;
            int cardValue = CardsValue[CardsSource[card.Value].SubstringUpToFirst('_')] ?? -1;

            return thisValue.CompareTo(cardValue);
        }
    }
}
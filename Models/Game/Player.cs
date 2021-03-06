﻿using System.Collections.Generic;

namespace InterWebs.Models.Game
{
    public class Player
    {
        private readonly List<Card> cards = new List<Card>();
        private string name = "";
        private int playedCardIndex = -1;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<Card> Cards
        {
            get { return cards; }
        }

        public int PlayedCardIndex
        {
            get { return playedCardIndex; }
            set { playedCardIndex = value; }
        }

        public int Id { get; set; }
    }
}
using System.Collections.Generic;

namespace InterWebs.Models.Game
{
    public class Player
    {
        private readonly List<int> cards = new List<int>();
        private string name = "";

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public List<int> Cards
        {
            get { return cards; }
        } 

        public int Id { get; set; }
    }
}
using System.Collections.Generic;
using CardGamesDomain;

namespace RummyGames
{
    public class Table
    {
        public Table(IEnumerable<Player> players, Deck deck)
        {
            Players = players;
            Deck = deck;
        }

        public IEnumerable<Player> Players { get; }
        public Deck Deck { get; }
    }
}
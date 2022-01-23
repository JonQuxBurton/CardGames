using System.Collections.Generic;
using CardGamesDomain;

namespace RummyGames
{
    public class Table
    {
        public Table(IEnumerable<Player> players, Deck deck, DiscardPile discardPile, StockPile stockPile = null)
        {
            Players = players;
            Deck = deck;
            DiscardPile = discardPile;
            StockPile = stockPile;
        }

        public IEnumerable<Player> Players { get; }
        public Deck Deck { get; }
        public DiscardPile DiscardPile { get; }
        public StockPile StockPile { get; }
    }
}
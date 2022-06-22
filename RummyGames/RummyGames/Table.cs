using System.Collections.Generic;
using CardGamesDomain;

namespace RummyGames
{
    public class Table
    {
        public Table(IEnumerable<Player> players, Deck deck, DiscardPile discardPile, StockPile stockPile = null,
            IEnumerable<IEnumerable<Card>> laydowns = null)
        {
            Players = players;
            Deck = deck;
            DiscardPile = discardPile;
            StockPile = stockPile;
            Laydowns = laydowns;
        }

        public IEnumerable<Player> Players { get; }
        public Deck Deck { get; }
        public DiscardPile DiscardPile { get; }
        public StockPile StockPile { get; }
        public IEnumerable<IEnumerable<Card>> Laydowns { get; }
    }
}
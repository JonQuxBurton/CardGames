using System;
using System.Collections.Generic;
using System.Linq;
using CardGamesDomain;

namespace RummyGames
{
    public class StockPile
    {
        public StockPile()
        {
            Cards = Array.Empty<Card>();
        }

        public StockPile(IEnumerable<Card> cards)
        {
            Cards = cards;
        }

        public IEnumerable<Card> Cards { get; }

        public Card TopCard => Cards.FirstOrDefault();
        public bool IsEmpty => !Cards.Any();
    }
}
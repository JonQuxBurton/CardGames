using System.Collections.Generic;
using System.Linq;
using CardGamesDomain;

namespace RummyGames
{
    public class DiscardPile
    {
        public DiscardPile()
        {
            Cards = System.Array.Empty<Card>();
        }

        public DiscardPile(params Card[] cards)
        {
            Cards = cards;
        }
        
        public DiscardPile(IEnumerable<Card> cards)
        {
            Cards = cards;
        }

        public IEnumerable<Card> Cards { get; }

        public Card TurnedUpCard => Cards.FirstOrDefault();
        public IEnumerable<Card> RestOfCards => Cards.Skip(1);
    }
}
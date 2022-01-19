using System.Collections.Generic;

namespace CardGamesDomain
{
    public class Deck
    {
        public Deck(IEnumerable<Card> cards)
        {
            Cards = cards;
        }

        public IEnumerable<Card> Cards { get; }
    }
}
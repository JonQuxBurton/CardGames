using System.Collections.Generic;
using System.Linq;

namespace CardGamesDomain
{
    public class Hand
    {
        public Hand()
        {
            Cards = System.Array.Empty<Card>();
        }

        public Hand(IEnumerable<Card> cards)
        {
            Cards = cards;
        }

        public IEnumerable<Card> Cards { get; }
        public bool IsLastCard => Cards.Count() == 1;

        public bool Contains(Card card)
        {
            return Cards.Any(x => x.Equals(card));
        }

        public override string ToString()
        {
            return string.Join(',', Cards.Select(x => x.ToString()));
        }
    }
}
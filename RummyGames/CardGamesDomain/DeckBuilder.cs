using System.Collections.Generic;

namespace CardGamesDomain
{
    public class DeckBuilder
    {
        private static readonly List<Rank> RankOrderAceLow = new()
        {
            Rank.ACE,
            Rank.TWO,
            Rank.THREE,
            Rank.FOUR,
            Rank.FIVE,
            Rank.SIX,
            Rank.SEVEN,
            Rank.EIGHT,
            Rank.NINE,
            Rank.TEN,
            Rank.JACK,
            Rank.QUEEN,
            Rank.KING
        };

        public Deck Build()
        {
            var cards = new List<Card>();
            for (var i = 1; i < 14; i++)
            {
                cards.Add(new Card(RankOrderAceLow[i - 1], Suit.CLUBS));
                cards.Add(new Card(RankOrderAceLow[i - 1], Suit.DIAMONDS));
                cards.Add(new Card(RankOrderAceLow[i - 1], Suit.HEARTS));
                cards.Add(new Card(RankOrderAceLow[i - 1], Suit.SPADES));
            }

            return new Deck(cards);
        }
    }
}
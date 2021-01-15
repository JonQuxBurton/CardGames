using System.Collections.Generic;

namespace SheddingCardGames
{
    public class DeckBuilder : IDeckBuilder
    {
        public CardCollection Build()
        {
            var suitSize = 13;
            var cards = new List<Card>();
            for (var i = 0; i < suitSize; i++)
                cards.Add(new Card(i + 1, Suit.Clubs));
            for (var i = 0; i < suitSize; i++)
                cards.Add(new Card(i + 1, Suit.Diamonds));
            for (var i = 0; i < suitSize; i++)
                cards.Add(new Card(i + 1, Suit.Hearts));
            for (var i = 0; i < suitSize; i++)
                cards.Add(new Card(i + 1, Suit.Spades));

            return new CardCollection(cards);
        }
    }
}

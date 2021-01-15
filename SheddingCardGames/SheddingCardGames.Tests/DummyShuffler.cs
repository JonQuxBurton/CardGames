using System.Collections.Generic;

namespace SheddingCardGames.Tests
{
    public class DummyShuffler: IShuffler
    {
        public IEnumerable<Card> Shuffle(IEnumerable<Card> cards)
        {
            return cards;
        }
    }
}
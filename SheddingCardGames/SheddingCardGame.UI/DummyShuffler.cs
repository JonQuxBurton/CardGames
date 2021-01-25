using System.Collections.Generic;
using SheddingCardGames;

namespace SheddingCardGame.UI
{
    public class DummyShuffler : IShuffler
    {
        public IEnumerable<Card> Shuffle(IEnumerable<Card> cards)
        {
            return cards;
        }
    }
}
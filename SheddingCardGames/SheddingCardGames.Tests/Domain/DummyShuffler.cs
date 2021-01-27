using System.Collections.Generic;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class DummyShuffler: IShuffler
    {
        public IEnumerable<Card> Shuffle(IEnumerable<Card> cards)
        {
            return cards;
        }
    }
}
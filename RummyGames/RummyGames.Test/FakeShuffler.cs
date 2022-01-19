using System.Collections;
using System.Collections.Generic;
using CardGamesDomain;

namespace RummyGames.Test
{
    public class FakeShuffler : IShuffler
    {
        private readonly IEnumerable expected;

        public FakeShuffler(IEnumerable expectedToReturn)
        {
            expected = expectedToReturn;
        }

        public IEnumerable<T> Shuffle<T>(IEnumerable<T> cardsToShuffle)
        {
            return (IEnumerable<T>) expected;
        }
    }
}
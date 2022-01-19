using System.Collections.Generic;

namespace CardGamesDomain
{
    public interface IShuffler
    {
        IEnumerable<T> Shuffle<T>(IEnumerable<T> cardsToShuffle);
    }
}
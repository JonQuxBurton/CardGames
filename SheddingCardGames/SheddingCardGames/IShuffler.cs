using System.Collections.Generic;

namespace SheddingCardGames
{
    public interface IShuffler
    {
        IEnumerable<Card> Shuffle(IEnumerable<Card> cards);
    }
}
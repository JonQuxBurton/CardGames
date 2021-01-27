using System.Collections.Generic;

namespace SheddingCardGames.Domain
{
    public interface IShuffler
    {
        IEnumerable<Card> Shuffle(IEnumerable<Card> cards);
    }
}
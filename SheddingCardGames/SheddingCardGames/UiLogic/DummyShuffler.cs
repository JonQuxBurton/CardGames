using System.Collections.Generic;
using SheddingCardGames;
using SheddingCardGames.Domain;

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
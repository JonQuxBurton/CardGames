using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SheddingCardGames
{
    [ExcludeFromCodeCoverage]
    public class Shuffler : IShuffler
    {
        private readonly Random random;

        public Shuffler()
        {
            random = new Random();
        }

        public IEnumerable<Card> Shuffle(IEnumerable<Card> cards)
        {
            return cards.ToList().OrderBy(x => random.Next());
        }
    }
}
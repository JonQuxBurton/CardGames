using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class Shuffler : IShuffler
    {
        private readonly Random random;

        public Shuffler()
        {
            random = new Random();
        }

        public CardCollection Shuffle(CardCollection cards)
        {
            return new CardCollection(cards.Cards.ToList().OrderBy(x => random.Next()));
        }
    }
}
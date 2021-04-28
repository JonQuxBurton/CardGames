using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class ValidPlays
    {
        public ValidPlays()
        {
        }

        public ValidPlays(IImmutableList<IImmutableList<Card>> plays)
        {
            Plays = plays;
        }

        public ValidPlays(IEnumerable<Card> singleCardPlays)
        {
            if (!singleCardPlays.Any())
                return;

            Plays = ImmutableList.Create<IImmutableList<Card>>()
                .Add(ImmutableList.Create(singleCardPlays.ToArray()));
        }

        public IImmutableList<IImmutableList<Card>> Plays { get; } = ImmutableList<IImmutableList<Card>>.Empty;

        public bool Any()
        {
            return Plays.Any();
        }
    }
}
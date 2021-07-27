using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class IsValidPlayContext
    {
        public IsValidPlayContext(IImmutableList<Card> cardsPlayed, Card discardCard, Suit? selectedSuit,
            bool anyPlaysOrTakes)
        {
            CardsPlayed = cardsPlayed;
            DiscardCard = discardCard;
            SelectedSuit = selectedSuit;
            AnyPlaysOrTakes = anyPlaysOrTakes;
        }

        public bool IsSingleCardPlayed => CardsPlayed.Count == 1;
        public Card FirstCardPlayed => CardsPlayed.First();
        public IEnumerable<Card> RestOfCards => CardsPlayed.Skip(1);

        public IImmutableList<Card> CardsPlayed { get; }
        public Card DiscardCard { get; }
        public Suit? SelectedSuit { get; }
        public bool AnyPlaysOrTakes { get; }
    }
}
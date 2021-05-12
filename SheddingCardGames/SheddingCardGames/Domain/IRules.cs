using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public interface IRules
    {
        bool HasValidPlay(Card discardCard, CardCollection hand, Suit? selectedSuit,
            bool anyPlaysOrTakes);
        bool IsValidPlay(IImmutableList<Card> cardsPlayed, Card discardCard, Suit? selectedSuit,
            bool anyPlaysOrTakes);
        int GetHandSize();
    }
}
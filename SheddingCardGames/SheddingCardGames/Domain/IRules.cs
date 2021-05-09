using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public interface IRules
    {
        bool HasValidPlay(Card discardCard, CardCollection hand, int turnNumber, Suit? selectedSuit);
        bool IsValidPlay(IImmutableList<Card> cardsPlayed, Card discardCard, int turnNumber, Suit? selectedSuit);
        int GetHandSize();
    }
}
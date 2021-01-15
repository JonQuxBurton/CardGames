using System.Collections.Generic;

namespace SheddingCardGames
{
    public interface IRules
    {
        IEnumerable<Card> GetValidPlays(Card discard, CardCollection hand, int turnNumber, Suit? selectedSuit);
        bool IsValidPlay(Card playedCard, Card discardCard, int turnNumber, Suit? selectedSuit);
        int GetHandSize();
    }
}
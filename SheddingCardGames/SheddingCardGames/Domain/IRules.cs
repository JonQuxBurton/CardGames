namespace SheddingCardGames.Domain
{
    public interface IRules
    {
        ValidPlays GetValidPlays(Card discard, CardCollection hand, int turnNumber,
            Suit? selectedSuit);

        bool IsValidPlay(Card playedCard, Card discardCard, int turnNumber, Suit? selectedSuit);
        int GetHandSize();
    }
}
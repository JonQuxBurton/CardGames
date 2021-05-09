using System.Collections.Immutable;
using System.Linq;
using static SheddingCardGames.Domain.CardsUtils;

namespace SheddingCardGames.Domain
{
    public class Rules : IRules
    {
        public Rules()
        {
            HandSize = 7;
        }

        public Rules(int handSize)
        {
            HandSize = handSize;
        }

        public int HandSize { get; }

        public bool HasValidPlay(Card discardCard, CardCollection hand, int turnNumber, Suit? selectedSuit)
        {
            return hand.Cards.Any(x => IsValidPlay(Cards(x), discardCard, turnNumber, selectedSuit));
        }

        public bool IsValidPlay(IImmutableList<Card> cardsPlayed, Card discardCard, int turnNumber, Suit? selectedSuit)
        {
            if (turnNumber == 1 && discardCard.Rank == 8)
                return true;

            if (discardCard.Rank == 8 && selectedSuit != null)
            {
                if (selectedSuit == cardsPlayed.First().Suit || cardsPlayed.First().Rank == 8)
                    return true;

                return false;
            }

            if (discardCard.Suit == cardsPlayed.First().Suit || discardCard.Rank == cardsPlayed.First().Rank ||
                cardsPlayed.First().Rank == 8)
                return true;

            return false;
        }

        public int GetHandSize()
        {
            return HandSize;
        }
    }
}
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

        public bool HasValidPlay(Card discardCard, CardCollection hand, Suit? selectedSuit,
            bool anyPlaysOrTakes)
        {
            return hand.Cards.Any(x => IsValidPlay(Cards(x), discardCard, selectedSuit, anyPlaysOrTakes));
        }

        public bool IsValidPlay(IImmutableList<Card> cardsPlayed, Card discardCard, Suit? selectedSuit,
            bool anyPlaysOrTakes)
        {
            if (!anyPlaysOrTakes && discardCard.Rank == 8)
                return true;

            if (discardCard.Rank == 8 && selectedSuit == null)
            {
                return false;
            }

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
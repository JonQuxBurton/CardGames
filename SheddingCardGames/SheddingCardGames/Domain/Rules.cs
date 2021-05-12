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

            return IsCardValid(cardsPlayed.First(), discardCard, selectedSuit);
        }

        private static bool IsCardValid(Card cardPlayed, Card discardCard, Suit? selectedSuit)
        {
            if (cardPlayed.Rank == 8)
                return true;

            if (discardCard.Rank == cardPlayed.Rank)
                return true;

            if (selectedSuit != null && selectedSuit == cardPlayed.Suit)
                return true;

            if (selectedSuit == null && discardCard.Suit == cardPlayed.Suit)
                return true;

            return false;
        }


        public int GetHandSize()
        {
            return HandSize;
        }
    }
}
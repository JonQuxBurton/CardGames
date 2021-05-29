using System.Collections.Immutable;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public abstract class CrazyEightsRules
    {
        public enum NumberOfPlayers
        {
            Two = 2,
            Three = 3,
            Four = 4
        }

        protected readonly int numberOfPlayers;

        protected CrazyEightsRules(NumberOfPlayers numberOfPlayers)
        {
            this.numberOfPlayers = (int)numberOfPlayers;
        }

        public bool HasValidPlay(Card discardCard, CardCollection hand, Suit? selectedSuit,
            bool anyPlaysOrTakes)
        {
            return hand.Cards.Any(x => IsValidPlay(CardsUtils.Cards(x), discardCard, selectedSuit, anyPlaysOrTakes));
        }

        public bool IsValidPlay(IImmutableList<Card> cardsPlayed, Card discardCard, Suit? selectedSuit,
            bool anyPlaysOrTakes)
        {
            if (!anyPlaysOrTakes && discardCard.Rank == 8)
                return true;

            return cardsPlayed.All(x => IsCardValid(x, discardCard, selectedSuit));
        }

        public int GetHandSize()
        {
            if (numberOfPlayers == 2)
                return 7;

            return 5;
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

        public abstract int NumberOfTakesBeforePass { get; }
    }
}
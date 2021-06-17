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
            return hand.Cards.Any(x => IsValidPlay(new IsValidPlayContext(CardsUtils.Cards(x), discardCard, selectedSuit, anyPlaysOrTakes)));
        }

        public bool IsValidPlay(IsValidPlayContext isValidPlayContext)
        {
            return IsFirstCardValid(isValidPlayContext) &&
                   AreRestOfCardsValid(isValidPlayContext);
        }

        private static bool IsFirstCardValid(IsValidPlayContext isValidPlayContext)
        {
            if (!isValidPlayContext.AnyPlaysOrTakes && isValidPlayContext.DiscardCard.Rank == 8)
                return true;

            return IsCardValid(isValidPlayContext.FirstCardPlayed, 
                isValidPlayContext);
        }

        private static bool AreRestOfCardsValid(IsValidPlayContext isValidPlayContext)
        {
            return isValidPlayContext.IsSingleCardPlayed || 
                   isValidPlayContext.RestOfCards.All(x => x.Rank == isValidPlayContext.FirstCardPlayed.Rank);
        }

        public int GetHandSize()
        {
            if (numberOfPlayers == 2)
                return 7;

            return 5;
        }

        private static bool IsCardValid(Card cardPlayed, IsValidPlayContext isValidPlayContext)
        {
            if (cardPlayed.Rank == 8)
                return true;

            if (isValidPlayContext.DiscardCard.Rank == cardPlayed.Rank)
                return true;

            if (isValidPlayContext.SelectedSuit != null && isValidPlayContext.SelectedSuit == cardPlayed.Suit)
                return true;

            if (isValidPlayContext.SelectedSuit == null && isValidPlayContext.DiscardCard.Suit == cardPlayed.Suit)
                return true;

            return false;
        }

        public abstract int NumberOfTakesBeforePass { get; }
    }
}
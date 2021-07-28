using System.Linq;

namespace SheddingCardGames.Domain.Whist
{
    public class Rules
    {
        public bool IsValidPlay(IsValidPlayContext isValidPlayContext)
        {
            var trickSuit = isValidPlayContext.CurrentStateOfTrick.TrickSuit;

            return trickSuit == null ||
                   CardPlayedMatchesTrickSuit(isValidPlayContext, trickSuit) ||
                   PlayerDoesNotHaveCardMatchingTrickSuit(isValidPlayContext, trickSuit);
        }

        private static bool CardPlayedMatchesTrickSuit(IsValidPlayContext isValidPlayContext, Suit? trickSuit)
        {
            return trickSuit == isValidPlayContext.CardPlayed.Suit;
        }

        private static bool PlayerDoesNotHaveCardMatchingTrickSuit(IsValidPlayContext isValidPlayContext,
            Suit? trickSuit)
        {
            return isValidPlayContext.PlayerHand.Cards.All(x => x.Suit != trickSuit);
        }
    }
}
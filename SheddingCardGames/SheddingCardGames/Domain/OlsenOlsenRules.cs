using System.Collections.Immutable;
using System.Linq;
using static SheddingCardGames.Domain.CardsUtils;

namespace SheddingCardGames.Domain
{
    public class OlsenOlsenRules : IRules
    {
        public OlsenOlsenRules()
        {
            HandSize = 7;
        }

        public OlsenOlsenRules(int handSize)
        {
            HandSize = handSize;
        }

        public int HandSize { get; }

        public int GetHandSize()
        {
            return HandSize;
        }

        public bool HasValidPlay(Card discardCard, CardCollection hand, int turnNumber, Suit? selectedSuit)
        {
            return hand.Cards.Any(x => IsValidPlay(Cards(x), discardCard, turnNumber, selectedSuit));
        }

        public bool IsValidPlay(IImmutableList<Card> cardsPlayed, Card discardCard, int turnNumber, Suit? selectedSuit)
        {
            if (turnNumber == 1 && discardCard.Rank == 8)
                return true;

            return cardsPlayed.All(x => IsCardValid(x, discardCard, selectedSuit));
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
    }
}
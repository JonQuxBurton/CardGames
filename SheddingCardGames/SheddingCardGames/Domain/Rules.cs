using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class Rules : IRules
    {
        public int HandSize { get; }

        public Rules()
        {
            HandSize = 7;
        }
        
        public Rules(int handSize)
        {
            HandSize = handSize;
        }
        
        public ValidPlays GetValidPlays(Card discard, CardCollection hand, int turnNumber, Suit? selectedSuit)
        {
            if (turnNumber == 1 && discard.Rank == 8)
                return new ValidPlays(hand.Cards.ToList());

            IEnumerable<Card> suitMatches;
            if (discard.Rank == 8)
                suitMatches = hand.Cards.Where(x => x.Suit == selectedSuit);
            else
                suitMatches = hand.Cards.Where(x => x.Suit == discard.Suit);

            var set = new HashSet<Card>();

            foreach (var match in suitMatches) 
                set.Add(match);

            foreach (var match in hand.Cards.Where(x => x.Rank == discard.Rank)) 
                set.Add(match);

            foreach (var match in hand.Cards.Where(x => x.Rank == 8)) 
                set.Add(match);

            return new ValidPlays(set.ToList());
            //return set.ToList();
        }

        public bool IsValidPlay(Card playedCard, Card discardCard, int turnNumber, Suit? selectedSuit)
        {
            if (turnNumber == 1 && discardCard.Rank == 8)
                return true;

            if (discardCard.Rank == 8 && selectedSuit != null)
            {
                if (selectedSuit == playedCard.Suit || playedCard.Rank == 8)
                    return true;
                
                return false;
            }
            
            if (discardCard.Suit == playedCard.Suit || discardCard.Rank == playedCard.Rank || playedCard.Rank == 8)
                return true;

            return false;
        }

        public int GetHandSize()
        {
            return HandSize;
        }
    }
}
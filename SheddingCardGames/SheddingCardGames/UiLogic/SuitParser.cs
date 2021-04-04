using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class SuitParser
    {
        public Suit Parse(string suitString)
        {
            var suit = Suit.Clubs;

            if (suitString == "D")
                suit = Suit.Diamonds;
            else if (suitString == "H")
                suit = Suit.Hearts;
            else if (suitString == "S")
                suit = Suit.Spades;
            return suit;
        }
    }
}
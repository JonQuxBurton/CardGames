using System.Linq;

namespace SheddingCardGames.Domain
{
    public class MinimalDeckBuilder : IDeckBuilder
    {
        public CardCollection Player1Hand = new CardCollection(
            new Card(1, Suit.Clubs),
            new Card(2, Suit.Clubs),
            new Card(3, Suit.Clubs),
            new Card(4, Suit.Clubs),
            new Card(5, Suit.Clubs),
            new Card(6, Suit.Clubs),
            new Card(7, Suit.Clubs)
        );
        
        public CardCollection Player2Hand = new CardCollection(
            new Card(1, Suit.Diamonds),
            new Card(2, Suit.Diamonds),
            new Card(3, Suit.Diamonds),
            new Card(4, Suit.Diamonds),
            new Card(5, Suit.Diamonds),
            new Card(6, Suit.Diamonds),
            new Card(7, Suit.Diamonds)
        );

        public Card DiscardCard = new Card(9, Suit.Clubs);

        public CardCollection StockPile = new CardCollection(
            new Card(10, Suit.Clubs),
            new Card(11, Suit.Clubs)
        );


        public CardCollection Build()
        {
            return new CardCollection(
                Player1Hand.Cards.ElementAt(0),
                Player2Hand.Cards.ElementAt(0),
                Player1Hand.Cards.ElementAt(1),
                Player2Hand.Cards.ElementAt(1),
                Player1Hand.Cards.ElementAt(2),
                Player2Hand.Cards.ElementAt(2),
                Player1Hand.Cards.ElementAt(3),
                Player2Hand.Cards.ElementAt(3),
                Player1Hand.Cards.ElementAt(4),
                Player2Hand.Cards.ElementAt(4),
                Player1Hand.Cards.ElementAt(5),
                Player2Hand.Cards.ElementAt(5),
                Player1Hand.Cards.ElementAt(6),
                Player2Hand.Cards.ElementAt(6),
                DiscardCard,
                StockPile.Cards.ElementAt(0),
                StockPile.Cards.ElementAt(1)
            );
        }
    }
}
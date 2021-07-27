using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class MinimalDeckBuilder : IDeckBuilder
    {
        public Card DiscardCard = new Card(9, Suit.Clubs);

        public List<CardCollection> PlayerHands = new List<CardCollection>();

        public CardCollection StockPile = new CardCollection(
            new Card(10, Suit.Clubs),
            new Card(11, Suit.Clubs)
        );

        public MinimalDeckBuilder(int numberOfPlayers)
        {
            if (numberOfPlayers == 3)
            {
                PlayerHands.Add(
                    new CardCollection(
                        new Card(1, Suit.Clubs),
                        new Card(2, Suit.Clubs),
                        new Card(3, Suit.Clubs),
                        new Card(4, Suit.Clubs),
                        new Card(5, Suit.Clubs)
                    ));
                PlayerHands.Add(
                    new CardCollection(
                        new Card(1, Suit.Diamonds),
                        new Card(2, Suit.Diamonds),
                        new Card(3, Suit.Diamonds),
                        new Card(4, Suit.Diamonds),
                        new Card(5, Suit.Diamonds)
                    ));
                PlayerHands.Add(
                    new CardCollection(
                        new Card(1, Suit.Spades),
                        new Card(2, Suit.Spades),
                        new Card(3, Suit.Spades),
                        new Card(4, Suit.Spades),
                        new Card(5, Suit.Spades)
                    ));
            }
            else
            {
                PlayerHands.Add(
                    new CardCollection(
                        new Card(1, Suit.Clubs),
                        new Card(2, Suit.Clubs),
                        new Card(3, Suit.Clubs),
                        new Card(4, Suit.Clubs),
                        new Card(5, Suit.Clubs),
                        new Card(6, Suit.Clubs),
                        new Card(7, Suit.Clubs)
                    ));
                PlayerHands.Add(
                    new CardCollection(
                        new Card(1, Suit.Diamonds),
                        new Card(2, Suit.Diamonds),
                        new Card(3, Suit.Diamonds),
                        new Card(4, Suit.Diamonds),
                        new Card(5, Suit.Diamonds),
                        new Card(6, Suit.Diamonds),
                        new Card(7, Suit.Diamonds)
                    ));
            }
        }

        public CardCollection Build()
        {
            var cards = new List<Card>();
            for (var i = 0; i < PlayerHands[0].Count(); i++)
                foreach (var playerHand in PlayerHands)
                    cards.Add(playerHand.Cards.ElementAt(i));

            cards.Add(DiscardCard);
            cards.Add(StockPile.Cards.ElementAt(0));
            cards.Add(StockPile.Cards.ElementAt(1));

            return new CardCollection(
                cards
            );
        }
    }
}
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using Xunit;

namespace SheddingCardGames.Tests.Domain.CrazyEights
{
    namespace SpecificDeckBuilderTests
    {
        public class BuildShould
        {
            [Fact]
            public void ReturnDeckForTwoPlayers()
            {
                var player1Hand = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Suit.Hearts),
                    new Card(2, Suit.Hearts),
                    new Card(3, Suit.Hearts),
                    new Card(4, Suit.Hearts),
                    new Card(5, Suit.Hearts)
                );
                var discardCard = new Card(1, Suit.Clubs);
                var stockPile = new CardCollection(
                    new Card(6, Suit.Diamonds),
                    new Card(6, Suit.Hearts)
                );

                var deck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand).Build();

                deck.Cards.Should().Equal(
                    player1Hand.Cards.ElementAt(0),
                    player2Hand.Cards.ElementAt(0),
                    player1Hand.Cards.ElementAt(1),
                    player2Hand.Cards.ElementAt(1),
                    player1Hand.Cards.ElementAt(2),
                    player2Hand.Cards.ElementAt(2),
                    player1Hand.Cards.ElementAt(3),
                    player2Hand.Cards.ElementAt(3),
                    player1Hand.Cards.ElementAt(4),
                    player2Hand.Cards.ElementAt(4),
                    discardCard,
                    stockPile.Cards.ElementAt(0),
                    stockPile.Cards.ElementAt(1)
                );
            }

            [Fact]
            public void ReturnDeckForThreePlayers()
            {
                var player1Hand = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Suit.Hearts),
                    new Card(2, Suit.Hearts),
                    new Card(3, Suit.Hearts),
                    new Card(4, Suit.Hearts),
                    new Card(5, Suit.Hearts)
                );
                var player3Hand = new CardCollection(
                    new Card(1, Suit.Hearts),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Spades),
                    new Card(5, Suit.Spades)
                );
                var discardCard = new Card(1, Suit.Clubs);
                var stockPile = new CardCollection(
                    new Card(6, Suit.Diamonds),
                    new Card(6, Suit.Hearts)
                );

                var deck =
                    new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand).Build();

                deck.Cards.Should().Equal(
                    player1Hand.Cards.ElementAt(0),
                    player2Hand.Cards.ElementAt(0),
                    player3Hand.Cards.ElementAt(0),
                    player1Hand.Cards.ElementAt(1),
                    player2Hand.Cards.ElementAt(1),
                    player3Hand.Cards.ElementAt(1),
                    player1Hand.Cards.ElementAt(2),
                    player2Hand.Cards.ElementAt(2),
                    player3Hand.Cards.ElementAt(2),
                    player1Hand.Cards.ElementAt(3),
                    player2Hand.Cards.ElementAt(3),
                    player3Hand.Cards.ElementAt(3),
                    player1Hand.Cards.ElementAt(4),
                    player2Hand.Cards.ElementAt(4),
                    player3Hand.Cards.ElementAt(4),
                    discardCard,
                    stockPile.Cards.ElementAt(0),
                    stockPile.Cards.ElementAt(1)
                );
            }
        }
    }
}
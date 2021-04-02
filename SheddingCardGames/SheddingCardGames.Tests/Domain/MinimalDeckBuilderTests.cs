using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace MinimalDeckBuilderTests
    {
        public class BuildShould
        {
            [Fact]
            public void ReturnMinimalDeckWhenTwoPlayers()
            {
                var sut = new MinimalDeckBuilder(2);

                var actual = sut.Build();

                actual.Cards.Should().Equal(
                    new Card(1, Suit.Clubs),
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Clubs),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Clubs),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Clubs),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Clubs),
                    new Card(5, Suit.Diamonds),
                    new Card(6, Suit.Clubs),
                    new Card(6, Suit.Diamonds),
                    new Card(7, Suit.Clubs),
                    new Card(7, Suit.Diamonds),

                    new Card(9, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(11, Suit.Clubs)
                );
            }
            
            [Fact]
            public void ReturnMinimalDeckWhenThreePlayers()
            {
                var sut = new MinimalDeckBuilder(3);

                var actual = sut.Build();

                actual.Cards.Should().Equal(
                    new Card(1, Suit.Clubs),
                    new Card(1, Suit.Diamonds),
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Clubs),
                    new Card(2, Suit.Diamonds),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Clubs),
                    new Card(3, Suit.Diamonds),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Clubs),
                    new Card(4, Suit.Diamonds),
                    new Card(4, Suit.Spades),
                    new Card(5, Suit.Clubs),
                    new Card(5, Suit.Diamonds),
                    new Card(5, Suit.Spades),

                    new Card(9, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(11, Suit.Clubs)
                );
            }
        }
    }
}

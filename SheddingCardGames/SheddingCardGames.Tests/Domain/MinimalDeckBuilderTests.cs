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
            public void ReturnMinimalDeck()
            {
                var sut = new MinimalDeckBuilder();

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
        }
    }
}

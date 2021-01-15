using FluentAssertions;
using Xunit;

namespace SheddingCardGames.Tests
{
    namespace CardMoveEventTests
    {
        public class ToStringShould
        {
            [Fact]
            public void ReturnAsString()
            {
                var sut = new CardMoveEvent(new Card(13, Suit.Clubs), CardMoveSources.StockPile,
                    CardMoveSources.Player1Hand);

                var actual = sut.ToString();

                actual.Should().Be($"[13 Clubs from {CardMoveSources.StockPile} to {CardMoveSources.Player1Hand}]");
            }
        }
    }
}

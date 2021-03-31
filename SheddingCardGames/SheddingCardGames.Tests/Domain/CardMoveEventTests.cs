using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace CardMoveEventTests
    {
        public class ToStringShould
        {
            [Fact]
            public void ReturnAsString()
            {
                var sut = new CardMoveEvent(new Card(13, Suit.Clubs), CardMoveSources.StockPile,
                    CardMoveSources.PlayerHand(1));

                var actual = sut.ToString();

                actual.Should().Be($"[13 Clubs from {CardMoveSources.StockPile} to {CardMoveSources.PlayerHand(1)}]");
            }
        }
    }
}

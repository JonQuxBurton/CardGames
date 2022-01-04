using System.Collections.Immutable;
using FluentAssertions;
using SheddingCardGames.Domain.Whist;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain.Whist
{
    namespace StateOfTrickTests
    {
        public class TrickSuitShould
        {
            [Fact]
            public void ReturnSuit_WhenCardPlayed()
            {
                var sampleData = new SampleData();
                var sut = new StateOfTrick(1, sampleData.Player1, sampleData.Player1,
                    ImmutableList.Create(new Play(sampleData.Player1, Card(1, Clubs))));

                sut.TrickSuit.Should().Be(Clubs);
            }

            [Fact]
            public void ReturnNull_WhenNoCardsPlayed()
            {
                var sampleData = new SampleData();
                var sut = new StateOfTrick(1, sampleData.Player1, sampleData.Player1);

                sut.TrickSuit.Should().BeNull();
            }

            [Fact]
            public void ReturnNull_WhenCardsPlayedIsEmpty()
            {
                var sampleData = new SampleData();
                var sut = new StateOfTrick(1, sampleData.Player1, sampleData.Player1, ImmutableList<Play>.Empty);

                sut.TrickSuit.Should().BeNull();
            }
        }
    }
}
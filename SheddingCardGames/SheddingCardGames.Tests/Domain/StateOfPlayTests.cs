using System.Collections.Immutable;
using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace StateOfPlayTests
    {
        public class AnyPlaysOrTakesShould
        {
            private readonly SampleData sampleData;

            public AnyPlaysOrTakesShould()
            {
                sampleData = new SampleData();
            }

            [Fact]
            public void ReturnFalse_WhenNoCurrentStateOfTurn()
            {
                var sut = new StateOfPlay(new GameState());

                var actual = sut.AnyPlaysOrTakes;

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrue_WhenNotFirstTurnNumber()
            {
                var gameState = new GameState
                {
                    CurrentStateOfTurn = new StateOfTurn(2, sampleData.Player1, Action.Play)
                };
                var sut = new StateOfPlay(gameState);

                var actual = sut.AnyPlaysOrTakes;

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenFirstTurn_ButSpecialCardPlayed()
            {
                var gameState = new GameState
                {
                    CurrentStateOfTurn = new StateOfTurn(1, sampleData.Player1, Action.SelectSuit, null, null,
                        ImmutableList.Create(Action.Play))
                };
                var sut = new StateOfPlay(gameState);

                var actual = sut.AnyPlaysOrTakes;

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalse_WhenFirstTurn()
            {
                var gameState = new GameState
                {
                    CurrentStateOfTurn = new StateOfTurn(1, sampleData.Player1, Action.Play)
                };
                var sut = new StateOfPlay(gameState);

                var actual = sut.AnyPlaysOrTakes;

                actual.Should().BeFalse();
            }
        }
    }
}
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace ChooseStartingPlayerCommandTests
    {
        public class IsValidShould
        {
            [Fact]
            public void ReturnIsSuccessTrue()
            {
                var sampleData = new SampleData();
                var sut = new ChooseStartingPlayerCommand(new ChooseStartingPlayerContext(sampleData.Player1));

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }
        }

        public class ExecuteShould
        {
            private readonly GameState actual;
            private readonly Player expectedPlayer;

            public ExecuteShould()
            {
                var sampleData = new SampleData();
                expectedPlayer = sampleData.Player2;
                var sut = new ChooseStartingPlayerCommand(new ChooseStartingPlayerContext(expectedPlayer));

                actual = sut.Execute();
            }

            [Fact]
            public void ReturnGameStateWithPlayerToStart()
            {
                actual.PlayerToStart.Should().Be(expectedPlayer);
            }

            [Fact]
            public void ReturnGameStatePreviousTurnResultNull()
            {
                actual.PreviousTurnResult.Should().BeNull();
            }
            
            [Fact]
            public void ReturnGameStateWithCurrentTurnNull()
            {
                actual.CurrentTurn.Should().BeNull();
            }

            [Fact]
            public void ReturnGameStateWithCurrentGamePhaseReadyToDeal()
            {
                actual.CurrentGamePhase.Should().Be(GamePhase.ReadyToDeal);
            }

            [Fact]
            public void AddStartingPlayerChosenEvent()
            {
                actual.Events.First().Should().BeOfType<StartingPlayerChosen>();
                var actualEvent = actual.Events.First() as StartingPlayerChosen;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.Player.Should().Be(expectedPlayer);
            }
        }
    }
}
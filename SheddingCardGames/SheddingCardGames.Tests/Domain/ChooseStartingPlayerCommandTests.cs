using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.PlayersUtils;

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
                var initialGameState = new GameState(Players(sampleData.Player1, sampleData.Player2));
                var sut = new ChooseStartingPlayerCommand(new DummyPlayerChooser(), initialGameState, new ChooseStartingPlayerContext());

                var actual = sut.IsValid();

                actual.IsValid.Should().BeTrue();
                actual.MessageKey.Should().Be(CommandExecutionResultMessageKey.Success);
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
                var initialGameState = new GameState(Players(sampleData.Player1, sampleData.Player2));
                var dummyPlayerChooser = new DummyPlayerChooser(expectedPlayer);
                var sut = new ChooseStartingPlayerCommand(dummyPlayerChooser, initialGameState, new ChooseStartingPlayerContext());

                actual = sut.Execute();
            }

            [Fact]
            public void ReturnGameStateWithPlayerToStart()
            {
                actual.PlayerToStart.Should().Be(expectedPlayer);
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
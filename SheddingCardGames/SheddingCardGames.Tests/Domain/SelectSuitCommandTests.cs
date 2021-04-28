using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace SelectSuitCommandTests
    {
        public class SelectSuitCommandBuilder
        {
            private readonly int turnNumber = 1;
            private DiscardPile discardPile = new DiscardPile();
            private int executingPlayerNumber = 1;
            private readonly CardCollection player1Hand = new CardCollection();
            private CardCollection player2Hand = new CardCollection();
            private int playerToPlayNumber = 1;
            private readonly CardCollection stockPile = new CardCollection();

            public SelectSuitCommandBuilder WithExecutingPlayer(int withExecutingPlayerNumber)
            {
                executingPlayerNumber = withExecutingPlayerNumber;
                return this;
            }

            public SelectSuitCommandBuilder WithPlayerToPlayNumber(int withPlayerToPlayNumber)
            {
                playerToPlayNumber = withPlayerToPlayNumber;
                return this;
            }

            public SelectSuitCommandBuilder WithDiscardPile(DiscardPile withDiscardPile)
            {
                discardPile = withDiscardPile;
                return this;
            }

            public SelectSuitCommandBuilder WithPlayer2Hand(CardCollection withPlayer2Hand)
            {
                player2Hand = withPlayer2Hand;
                return this;
            }

            public SelectSuitCommand Build()
            {
                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;

                var playerToPlay = player1;
                if (playerToPlayNumber == 2)
                    playerToPlay = player2;

                var executingPlayer = player1;
                if (executingPlayerNumber == 2)
                    executingPlayer = player2;

                discardPile.TurnUpTopCard();
                var table = TableCreator.Create(new StockPile(stockPile), discardPile, player1, player2);
                var gameState = new GameState
                {
                    CurrentTable = table,
                    PlayerToStart = player1,
                    CurrentTurn = new CurrentTurn(turnNumber, playerToPlay, new ValidPlays(), Action.Play)
                };

                return new SelectSuitCommand(new Rules(), gameState, new SelectSuitContext(executingPlayer, Suit.Hearts));
            }
        }

        public class IsValidShould
        {
            [Fact]
            public void ReturnIsSuccessTrue()
            {
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(8, Suit.Hearts)
                ));
                var sut = new SelectSuitCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .Build();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenNotPlayersTurn()
            {
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(8, Suit.Hearts)
                ));
                var sut = new SelectSuitCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithExecutingPlayer(2)
                    .WithPlayerToPlayNumber(1)
                    .Build();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenCardPlayedWasNotAnEight()
            {
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(2, Suit.Clubs)
                ));
                var sut = new SelectSuitCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .Build();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }
        }

        public class ExecuteShould
        {
            private readonly GameState actual;
            private readonly Suit selectedSuit;

            public ExecuteShould()
            {
                selectedSuit = Suit.Hearts;
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(8, Suit.Clubs)
                ));
                var player2Hand = new CardCollection(new Card(1, Suit.Hearts));
                var sut = new SelectSuitCommandBuilder()
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build();

                actual = sut.Execute();
            }
            
            [Fact]
            public void AddSuitSelectedEvent()
            {
                actual.Events.First().Should().BeOfType<SuitSelected>();
                var actualEvent = actual.Events.First() as SuitSelected;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.PlayerNumber.Should().Be(1);
                actualEvent.Suit.Should().Be(selectedSuit);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.ValidPlays.Plays.First().Should().BeEquivalentTo(new Card(1, Suit.Hearts));
                actualTurn.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = actual.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();
                actualPreviousTurnResult.SelectedSuit.Should().Be(selectedSuit);
                actualPreviousTurnResult.TakenCard.Should().BeNull();
            }
        }
    }
}
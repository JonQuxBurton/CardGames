using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules.NumberOfPlayers;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain
{
    namespace SelectSuitCommandTests
    {
        public class SelectSuitCommandBuilder
        {
            private readonly CardCollection player1Hand = new CardCollection();
            private readonly CardCollection stockPile = new CardCollection();
            private DiscardPile discardPile = new DiscardPile();
            private int executingPlayerNumber = 1;
            private CardCollection player2Hand = new CardCollection();
            private int playerToPlayNumber = 1;
            private Suit selectedSuit;
            private readonly int turnNumber = 1;
            private readonly List<Action> previousActions = new List<Action>();

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

            public SelectSuitCommandBuilder WithSelectedSuit(Suit withSelectedSuit)
            {
                selectedSuit = withSelectedSuit;
                return this;
            }

            public SelectSuitCommand BuildWithPlay()
            {
                previousActions.Add(Action.Play);

                return Build();
            }

            public SelectSuitCommand BuildWithNoPlaysOrTakes()
            {
                return Build();
            }

            private SelectSuitCommand Build()
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
                var players = Players(sampleData.Player1, sampleData.Player2);
                var table = TableCreator.Create(new StockPile(stockPile), discardPile, players);
                var gameState = new GameState(players)
                {
                    CurrentTable = table,
                    PlayerToStart = player1,
                    CurrentTurn = new CurrentTurn(turnNumber, playerToPlay, Action.Play, null, null, null,
                        ImmutableList.Create(previousActions.ToArray()))
                };

                return new SelectSuitCommand(new CrazyEightsRules(Two), gameState,
                    new SelectSuitContext(executingPlayer, selectedSuit));
            }
        }

        public class IsValidShould
        {
            private readonly Suit expectedSelectedSuit = Hearts;

            [Fact]
            public void ReturnIsSuccessTrue()
            {
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Hearts)
                ));
                var sut = new SelectSuitCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithSelectedSuit(expectedSelectedSuit)
                    .BuildWithPlay();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnIsSuccessFalse_WhenNotPlayersTurn()
            {
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Hearts)
                ));
                var sut = new SelectSuitCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithExecutingPlayer(2)
                    .WithPlayerToPlayNumber(1)
                    .WithSelectedSuit(expectedSelectedSuit)
                    .BuildWithPlay();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnIsSuccessFalse_WhenCardPlayedWasNotAnEight()
            {
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Clubs)
                ));
                var sut = new SelectSuitCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithSelectedSuit(expectedSelectedSuit)
                    .BuildWithPlay();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void ReturnIsSuccessFalse_WhenNoPlaysOrTakes()
            {
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Hearts)
                ));
                var sut = new SelectSuitCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithSelectedSuit(expectedSelectedSuit)
                    .BuildWithNoPlaysOrTakes();

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeFalse();

                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }
        }

        public class ExecuteShould
        {
            private readonly Suit expectedSelectedSuit;
            private readonly SelectSuitCommand sut;

            public ExecuteShould()
            {
                expectedSelectedSuit = Hearts;
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Clubs)
                ));
                var player2Hand = new CardCollection(new Card(1, Hearts));
                sut = new SelectSuitCommandBuilder()
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .WithSelectedSuit(expectedSelectedSuit)
                    .BuildWithPlay();
            }

            [Fact]
            public void AddSuitSelectedEvent()
            {
                var actual = sut.Execute();

                actual.Events.First().Should().BeOfType<SuitSelected>();
                var actualEvent = actual.Events.First() as SuitSelected;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.PlayerNumber.Should().Be(1);
                actualEvent.Suit.Should().Be(expectedSelectedSuit);
            }

            [Fact]
            public void AddSuitSelectedToGameState()
            {
                var actual = sut.Execute();

                actual.CurrentSelectedSuit.Should().Be(expectedSelectedSuit);
            }

            [Fact]
            public void CreateNewTurn_WithNextActionPlay()
            {
                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.CurrentAction.Should().Be(Action.Play);

                actualTurn.HasWinner.Should().BeFalse();
                actualTurn.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().Be(expectedSelectedSuit);
                actualTurn.TakenCard.Should().BeNull();
            }

            [Fact]
            public void CreateNewTurn_WithNextActionTake()
            {
                var selectedSuit = Hearts;
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Clubs)
                ));
                var player2Hand = new CardCollection(new Card(1, Clubs));
                var sut = new SelectSuitCommandBuilder()
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .WithSelectedSuit(expectedSelectedSuit)
                    .BuildWithPlay();

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.CurrentAction.Should().Be(Action.Take);

                actualTurn.HasWinner.Should().BeFalse();
                actualTurn.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().Be(selectedSuit);
                actualTurn.TakenCard.Should().BeNull();
            }
        }
    }
}
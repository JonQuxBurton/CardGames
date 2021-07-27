using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using SheddingCardGames.Domain.Events;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEights.CrazyEightsRules.NumberOfPlayers;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain.CrazyEights
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
                var gameState = new GameState
                {
                    GameSetup = new GameSetup(players)
                };
                gameState.GameSetup.WithStartingPlayer(player1);
                gameState.CurrentTable = table;
                gameState.CurrentStateOfTurn = new StateOfTurn(turnNumber, playerToPlay, Action.Play, null, selectedSuit, ImmutableList.Create(previousActions.ToArray()));
                gameState.CurrentStateOfPlay = new StateOfPlay();

                return new SelectSuitCommand(new BasicVariantRules(Two), gameState,
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

                actual.IsValid.Should().BeTrue();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.Success);
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

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.NotPlayersTurn);
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

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.InvalidPlay);
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
                actual.IsValid.Should().BeFalse();

                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.InvalidPlay);
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
            public void AddSuitSelectedEventAndTurnEndedEvent()
            {
                var actual = sut.Execute();

                var actualEvent = actual.EventLog.Events.First();
                actualEvent.Should().BeOfType<SuitSelected>();
                var suitSelectedEvent = actualEvent as SuitSelected;
                if (suitSelectedEvent == null) Assert.NotNull(suitSelectedEvent);
                suitSelectedEvent.Number.Should().Be(1);
                suitSelectedEvent.PlayerNumber.Should().Be(1);
                suitSelectedEvent.Suit.Should().Be(expectedSelectedSuit);
                
                actualEvent = actual.EventLog.Events.Last();
                actualEvent.Should().BeOfType<TurnEnded>();
                var turnEndedEvent = actualEvent as TurnEnded;
                if (turnEndedEvent == null) Assert.NotNull(turnEndedEvent);
                turnEndedEvent.Number.Should().Be(2);
                turnEndedEvent.PlayerNumber.Should().Be(1);
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

                var actualTurn = actual.CurrentStateOfTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.CurrentAction.Should().Be(Action.Play);

                actual.CurrentStateOfPlay.HasWinner.Should().BeFalse();
                actual.CurrentStateOfPlay.Winner.Should().BeNull();
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

                var actualTurn = actual.CurrentStateOfTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.CurrentAction.Should().Be(Action.Take);

                actual.CurrentStateOfPlay.HasWinner.Should().BeFalse();
                actual.CurrentStateOfPlay.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().Be(selectedSuit);
                actualTurn.TakenCard.Should().BeNull();
            }
        }
    }
}
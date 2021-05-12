using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain
{
    namespace TakeCommandTests
    {
        public class TakeCommandBuilder
        {
            private readonly int turnNumber = 1;
            private DiscardPile discardPile = new DiscardPile();
            private int executingPlayerNumber = 1;
            private CardCollection player1Hand = new CardCollection();
            private CardCollection player2Hand = new CardCollection();
            private int playerToPlayNumber = 1;
            private CardCollection stockPile = new CardCollection();
            private Suit? selectedSuit;

            public TakeCommandBuilder WithExecutingPlayer(int withExecutingPlayerNumber)
            {
                executingPlayerNumber = withExecutingPlayerNumber;
                return this;
            }

            public TakeCommandBuilder WithPlayerToPlayNumber(int withPlayerToPlayNumber)
            {
                playerToPlayNumber = withPlayerToPlayNumber;
                return this;
            }

            public TakeCommandBuilder WithPlayer1Hand(CardCollection withPlayer1Hand)
            {
                player1Hand = withPlayer1Hand;
                return this;
            }

            public TakeCommandBuilder WithPlayer2Hand(CardCollection withPlayer2Hand)
            {
                player2Hand = withPlayer2Hand;
                return this;
            }

            public TakeCommandBuilder WithDiscardPile(DiscardPile withDiscardPile)
            {
                discardPile = withDiscardPile;
                return this;
            }

            public TakeCommandBuilder WithStockPile(CardCollection withStockPile)
            {
                stockPile = withStockPile;
                return this;
            }

            public TakeCommandBuilder WithSelectedSuit(Suit withSelectedSuit)
            {
                selectedSuit = withSelectedSuit;
                return this;
            }
            
            public TakeCommand Build()
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
                    CurrentTurn = new CurrentTurn(turnNumber, playerToPlay, Action.Play),
                };

                if (selectedSuit.HasValue)
                    gameState.PreviousTurnResult = new PreviousTurnResult(false, null, selectedSuit);

                return new TakeCommand(new Rules(), new DummyShuffler(), gameState, new TakeContext(executingPlayer));
            }
        }

        public class IsValidShould
        {
            [Fact]
            public void ReturnIsSuccessTrueWhenNoValidPlays()
            {
                var player1Hand = new CardCollection(Card(1, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithDiscardPile(discardPile)
                    .Build();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenValidPlays()
            {
                var player1Hand = new CardCollection(Card(1, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Clubs)
                ));
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithDiscardPile(discardPile)
                    .Build();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenNotPlayersTurn()
            {
                var player1Hand = new CardCollection(Card(1, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Clubs)
                ));
                var sut = new TakeCommandBuilder()
                    .WithExecutingPlayer(2)
                    .WithPlayerToPlayNumber(1)
                    .WithPlayer1Hand(player1Hand)
                    .WithDiscardPile(discardPile)
                    .Build();

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }
        }

        public class ExecuteShould
        {
            private GameState actual;
            private Card takenCard;

            public ExecuteShould()
            {
                takenCard = Card(1, Hearts);
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var stockPile = new CardCollection(
                    takenCard
                );
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardPile(discardPile)
                    .Build();

                actual = sut.Execute();
            }

            [Fact]
            public void ReturnGameStateWithUpdatedTable()
            {
                actual.CurrentTable.StockPile.Cards.Should().NotContain(takenCard);
                actual.CurrentTable.Players[0].Hand.Cards.Should().Contain(takenCard);
            }

            [Fact]
            public void AddTakenEvent()
            {
                actual.Events.First().Should().BeOfType<Taken>();
                var actualEvent = actual.Events.First() as Taken;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.PlayerNumber.Should().Be(1);
                actualEvent.Card.Should().Be(takenCard);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = actual.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();
                actualPreviousTurnResult.SelectedSuit.Should().BeNull();
                actualPreviousTurnResult.TakenCard.Should().Be(takenCard);
            }
            
            [Fact]
            public void CreateNewTurn_WithSelectedSuitPreserved()
            {
                var expectedSelectedSuit = Spades;
                takenCard = new Card(1, Hearts);
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var stockPile = new CardCollection(
                    takenCard
                );
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardPile(discardPile)
                    .WithSelectedSuit(expectedSelectedSuit)
                    .Build();

                actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = actual.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();
                actualPreviousTurnResult.SelectedSuit.Should().Be(expectedSelectedSuit);
                actualPreviousTurnResult.TakenCard.Should().Be(takenCard);
            }
        }

        public class ExecuteWhenStockPileExhaustedShould
        {
            private readonly GameState actual;

            public ExecuteWhenStockPileExhaustedShould()
            {
                var takenCard = Card(1, Hearts);
                var player1Hand = new CardCollection(Card(1, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts),
                    Card(3, Clubs)
                ));
                var stockPile = new CardCollection(
                    takenCard
                );
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardPile(discardPile)
                    .Build();

                actual = sut.Execute();
            }

            [Fact]
            public void AddEvents()
            {
                actual.Events.ElementAt(1).Should().BeOfType<CardMoved>();
                var actualCardMovedEvent = actual.Events.ElementAt(1) as CardMoved;
                if (actualCardMovedEvent == null) Assert.NotNull(actualCardMovedEvent);
                actualCardMovedEvent.Number.Should().Be(2);
                actualCardMovedEvent.Card.Should().Be(Card(3, Clubs));
                actualCardMovedEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                actualCardMovedEvent.ToSource.Should().Be(CardMoveSources.StockPile);

                actual.Events.ElementAt(2).Should().BeOfType<Shuffled>();
                var actualShuffledEvent = actual.Events.ElementAt(2) as Shuffled;
                if (actualShuffledEvent == null) Assert.NotNull(actualShuffledEvent);
                actualShuffledEvent.Number.Should().Be(3);
                actualShuffledEvent.Target.Should().Be(CardMoveSources.StockPile);
                actualShuffledEvent.StartCards.Cards.Should().Equal(Card(3, Clubs));
                actualShuffledEvent.EndCards.Cards.Should().Equal(Card(3, Clubs));
            }

            [Fact]
            public void ReturnUpdatedTable()
            {
                actual.CurrentTable.StockPile.Cards.Should().Equal(Card(3, Clubs));
                actual.CurrentTable.DiscardPile.AllCards.Cards.Should().Equal(Card(2, Hearts));
            }
        }
    }
}
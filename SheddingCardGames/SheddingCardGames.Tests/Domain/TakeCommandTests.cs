using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;

// ReSharper disable InconsistentNaming

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
            private CrazyEightsRules rules = new BasicVariantRules(NumberOfPlayers.Two);
            private Suit? selectedSuit;
            private CardCollection stockPile = new CardCollection();

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

            public TakeCommandBuilder WithRules(CrazyEightsRules withRules)
            {
                rules = withRules;
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
                var players = Players(sampleData.Player1, sampleData.Player2);
                var table = TableCreator.Create(new StockPile(stockPile), discardPile, players);
                var gameState = new GameState(players)
                {
                    CurrentTable = table,
                    PlayerToStart = player1,
                    CurrentTurn = new CurrentTurn(turnNumber, playerToPlay, Action.Play, null, selectedSuit)
                };

                return new TakeCommand(rules, new DummyShuffler(), gameState, new TakeContext(executingPlayer));
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

                actual.IsValid.Should().BeTrue();
                actual.MessageKey.Should().Be(CommandExecutionResultMessageKey.Success);
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

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandExecutionResultMessageKey.InvalidTake);
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

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandExecutionResultMessageKey.NotPlayersTurn);
            }
        }

        public class Execute_WhenBasicVariant_Should
        {
            private GameState actual;
            private Card takenCard;

            public Execute_WhenBasicVariant_Should()
            {
                takenCard = Card(1, Hearts);
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var stockPile = new CardCollection(
                    takenCard,
                    Card(2, Hearts)
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
                var actualEvent = actual.Events.First();
                actualEvent.Should().BeOfType<Taken>();
                var domainEvent = actualEvent as Taken;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(1);
                domainEvent.PlayerNumber.Should().Be(1);
                domainEvent.Card.Should().Be(takenCard);
            }

            [Fact]
            public void AddPassedEvent()
            {
                var actualEvent = actual.Events.ElementAt(1);
                actualEvent.Should().BeOfType<Passed>();
                var domainEvent = actualEvent as Passed;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(2);
                domainEvent.PlayerNumber.Should().Be(1);
            }

            [Fact]
            public void AddTurnEndedEvent()
            {
                var actualEvent = actual.Events.ElementAt(2);
                actualEvent.Should().BeOfType<TurnEnded>();
                var domainEvent = actualEvent as TurnEnded;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(3);
                domainEvent.PlayerNumber.Should().Be(1);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);

                actualTurn.HasWinner.Should().BeFalse();
                actualTurn.Winner.Should().BeNull();
            }

            [Fact]
            public void UpdateCurrentTurnWithTakenCard()
            {
                actual.CurrentTurn.TakenCard.Should().Be(takenCard);
            }

            [Fact]
            public void UpdateCurrentTurnWithCurrentActionPlay()
            {
                actual.CurrentTurn.CurrentAction.Should().Be(Action.Play);
            }

            [Fact]
            public void CreateNewTurn_WithSelectedSuitPreserved_WhenCardTakenIsNotPlayable()
            {
                var expectedSelectedSuit = Spades;
                takenCard = Card(1, Spades);
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(1, Diamonds));
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
                actualTurn.CurrentAction.Should().Be(Action.Take);

                actualTurn.HasWinner.Should().BeFalse();
                actualTurn.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().Be(expectedSelectedSuit);
                actualTurn.TakenCard.Should().Be(takenCard);
            }

            [Fact]
            public void CreateTurn_WithSelectedSuitPreserved_WhenCardTakenIsPlayable()
            {
                var expectedSelectedSuit = Spades;
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Diamonds));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var stockPile = new CardCollection(
                    Card(1, Spades)
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
                actualTurn.CurrentAction.Should().Be(Action.Play);
                actualTurn.SelectedSuit.Should().Be(expectedSelectedSuit);
            }
        }

        public class Execute_WhenOlsenOlsenVariant_Should
        {
            private GameState actual;
            private Card takenCard;

            public Execute_WhenOlsenOlsenVariant_Should()
            {
                takenCard = Card(1, Hearts);
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var stockPile = new CardCollection(
                    takenCard,
                    Card(3, Hearts)
                );
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardPile(discardPile)
                    .WithRules(new OlsenOlsenVariantRules(NumberOfPlayers.Two))
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
                var actualEvent = actual.Events.Last();
                actualEvent.Should().BeOfType<Taken>();
                var domainEvent = actualEvent as Taken;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(1);
                domainEvent.PlayerNumber.Should().Be(1);
                domainEvent.Card.Should().Be(takenCard);
            }

            [Fact]
            public void NotCreateNewTurn()
            {
                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);

                actualTurn.HasWinner.Should().BeFalse();
                actualTurn.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().BeNull();
            }

            [Fact]
            public void UpdateCurrentTurnWithTakenCard()
            {
                actual.CurrentTurn.TakenCard.Should().Be(takenCard);
            }

            [Fact]
            public void UpdateCurrentTurnWithCurrentActionPlay()
            {
                actual.CurrentTurn.CurrentAction.Should().Be(Action.Play);
            }

            [Fact]
            public void UpdateCurrentTurnWithPreviousActionTake()
            {
                actual.CurrentTurn.PreviousActions.Should().Equal(Action.Take);
            }

            [Fact]
            public void UpdateCurrentTurnWithPreviousActionTakeTwice()
            {
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var stockPile = new CardCollection(
                    new Card(3, Spades),
                    new Card(4, Spades)
                );
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardPile(discardPile)
                    .WithRules(new OlsenOlsenVariantRules(NumberOfPlayers.Two))
                    .Build();

                sut.Execute();
                actual = sut.Execute();

                actual.CurrentTurn.PreviousActions.Should().Equal(Action.Take, Action.Take);
            }

            [Fact]
            public void UpdateTurn_WithSelectedSuitPreserved_WhenCardTakenIsNotPlayable()
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
                    .WithRules(new OlsenOlsenVariantRules(NumberOfPlayers.Two))
                    .Build();

                actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.CurrentAction.Should().Be(Action.Take);

                actualTurn.HasWinner.Should().BeFalse();
                actualTurn.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().Be(expectedSelectedSuit);
                actualTurn.TakenCard.Should().Be(takenCard);
            }

            [Fact]
            public void UpdateTurn_WithSelectedSuitPreserved_WhenCardTakenIsPlayable()
            {
                var expectedSelectedSuit = Spades;
                takenCard = new Card(10, Spades);
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
                    .WithRules(new OlsenOlsenVariantRules(NumberOfPlayers.Two))
                    .Build();

                actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.CurrentAction.Should().Be(Action.Play);
                actualTurn.SelectedSuit.Should().Be(expectedSelectedSuit);
            }
        }

        public class Execute_WhenOlsenOlsenVariant_AndWhenTakenThreeTimes_Should
        {
            private readonly GameState actual;

            public Execute_WhenOlsenOlsenVariant_AndWhenTakenThreeTimes_Should()
            {
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Clubs));
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Hearts)
                ));
                var stockPile = new CardCollection(
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades),
                    Card(6, Spades)
                );
                var sut = new TakeCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardPile(discardPile)
                    .WithRules(new OlsenOlsenVariantRules(NumberOfPlayers.Two))
                    .Build();

                sut.Execute();
                sut.Execute();
                actual = sut.Execute();
            }

            [Fact]
            public void StartNextTurn()
            {
                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);

                actualTurn.HasWinner.Should().BeFalse();
                actualTurn.Winner.Should().BeNull();
                actualTurn.TakenCard.Should().Be(Card(5, Spades));
            }

            [Fact]
            public void AddPassedEvent()
            {
                var actualEvent = actual.Events.ElementAt(3);
                actualEvent.Should().BeOfType<Passed>();
                var domainEvent = actualEvent as Passed;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(4);
                domainEvent.PlayerNumber.Should().Be(1);
            }

            [Fact]
            public void AddTurnEndedEvent()
            {
                var actualEvent = actual.Events.ElementAt(4);
                actualEvent.Should().BeOfType<TurnEnded>();
                var domainEvent = actualEvent as TurnEnded;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(5);
                domainEvent.PlayerNumber.Should().Be(1);
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
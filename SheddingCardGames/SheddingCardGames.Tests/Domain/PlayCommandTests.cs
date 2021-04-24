using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace PlayCommandTests
    {
        public class PlayCommandBuilder
        {
            private DiscardPile discardPile = new DiscardPile();
            private int executingPlayerNumber = 1;
            private CardCollection player1Hand = new CardCollection();
            private CardCollection player2Hand = new CardCollection();
            private readonly int turnNumber = 1;

            public PlayCommandBuilder WithExecutingPlayer(int withExecutingPlayerNumber)
            {
                executingPlayerNumber = withExecutingPlayerNumber;
                return this;
            }

            public PlayCommandBuilder WithPlayer1Hand(CardCollection withPlayer1Hand)
            {
                player1Hand = withPlayer1Hand;
                return this;
            }

            public PlayCommandBuilder WithPlayer2Hand(CardCollection withPlayer2Hand)
            {
                player2Hand = withPlayer2Hand;
                return this;
            }

            public PlayCommandBuilder WithDiscardPile(DiscardPile withDiscardPile)
            {
                discardPile = withDiscardPile;
                return this;
            }

            public PlayCommand Build(Card playedCard)
            {
                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var currentPlayer = player1;
                var executingPlayer = player1;
                if (executingPlayerNumber == 2)
                    executingPlayer = player2;

                discardPile.TurnUpTopCard();
                var table = TableCreator.Create(new StockPile(new CardCollection()), discardPile, player1, player2);
                var gameState = new GameState
                {
                    CurrentTable = table,
                    PlayerToStart = player1,
                    CurrentTurn = new CurrentTurn(turnNumber, currentPlayer, new Card[0], Action.Play)
                };

                return new PlayCommand(executingPlayer, new Rules(), gameState, playedCard);
            }
        }

        public class IsValidShould
        {
            [Fact]
            public void ReturnIsSuccessTrueWhenValid()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(2, Suit.Clubs)
                ));
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenCardIsNotInPlayersHand()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection();
                var player2Hand = new CardCollection();
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenNotPlayersTurn()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var discardPile = new DiscardPile(
                    new[] {new Card(1, Suit.Clubs)}
                );
                var player1Hand = new CardCollection();
                var player2Hand = new CardCollection(
                    playedCard
                );
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithExecutingPlayer(2)
                    .Build(playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnIsSuccessFalseForInvalidPlay()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(2, Suit.Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingSuit()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(1, Suit.Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingSuitWhenSelectedSuit()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(8, Suit.Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingRank()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(1, Suit.Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithRank8()
            {
                var playedCard = new Card(8, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(1, Suit.Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForAnyCardWhenFirstTurnAndDiscardCardIs8()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(8, Suit.Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenFirstTurnAndDiscardCardIsNot8()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(7, Suit.Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }
        }

        public class ExecuteShould
        {
            [Fact]
            public void ReturnUpdatedTable()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    new Card(2, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(new Card(1, Suit.Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(new Card(1, Suit.Clubs));
            }

            [Fact]
            public void AddPlayedEvent()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    new Card(2, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                actual.Events.Last().Should().BeOfType<Played>();
                var actualEvent = actual.Events.Last() as Played;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.PlayerNumber.Should().Be(1);
                actualEvent.Card.Should().Be(playedCard);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    new Card(2, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.ValidPlays.Should().BeEquivalentTo(new Card(1, Suit.Diamonds));
                actualTurn.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = actual.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();
                actualPreviousTurnResult.SelectedSuit.Should().BeNull();
                actualPreviousTurnResult.TakenCard.Should().BeNull();

                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(playedCard);
                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard);
            }

            [Fact]
            public void CreateNewTurnWithNextActionTake()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    new Card(2, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(10, Suit.Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.ValidPlays.Should().BeEmpty();
                actualTurn.NextAction.Should().Be(Action.Take);

                var actualPreviousTurnResult = actual.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();
                actualPreviousTurnResult.SelectedSuit.Should().BeNull();
                actualPreviousTurnResult.TakenCard.Should().BeNull();
            }

            [Fact]
            public void CreateWinningTurn()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(
                    playedCard
                );
                var player2Hand = new CardCollection(
                    new Card(10, Suit.Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.ValidPlays.Should().BeEmpty();
                actualTurn.NextAction.Should().Be(Action.Won);

                var actualPreviousTurnResult = actual.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeTrue();
                actualPreviousTurnResult.Winner.Number.Should().Be(1);
                actualPreviousTurnResult.SelectedSuit.Should().BeNull();
                actualPreviousTurnResult.TakenCard.Should().BeNull();
            }

            [Fact]
            public void CreateCrazyEightTurn()
            {
                var playedCard = new Card(8, Suit.Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    new Card(1, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(10, Suit.Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.ValidPlays.Should().BeEmpty();
                actualTurn.NextAction.Should().Be(Action.SelectSuit);

                var actualPreviousTurnResult = actual.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();
                actualPreviousTurnResult.SelectedSuit.Should().BeNull();
                actualPreviousTurnResult.TakenCard.Should().BeNull();

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard);
            }
        }
    }
}
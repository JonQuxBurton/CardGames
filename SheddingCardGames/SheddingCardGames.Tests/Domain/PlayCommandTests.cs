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
    namespace PlayCommandTests
    {
        public class PlayCommandBuilder
        {
            private DiscardPile discardPile = new DiscardPile();
            private int executingPlayerNumber = 1;
            private CardCollection player1Hand = new CardCollection();
            private CardCollection player2Hand = new CardCollection();
            private readonly int turnNumber = 1;
            private Suit? selectedSuit;

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

            public PlayCommandBuilder WithSelectedSuit(Suit withSelectedSuit)
            {
                selectedSuit = withSelectedSuit;
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
                    CurrentTurn = new CurrentTurn(turnNumber, currentPlayer, Action.Play),
                };

                if (selectedSuit != null)
                    gameState.PreviousTurnResult = new PreviousTurnResult(false, null, selectedSuit);

                return new PlayCommand(new Rules(), gameState,  new PlayContext(executingPlayer, Cards(playedCard)));
            }
        }

        public class IsValidShould
        {
            [Fact]
            public void ReturnIsSuccessTrueWhenValid()
            {
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Clubs)
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
                var playedCard = Card(1, Clubs);
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
                var playedCard = Card(1, Clubs);
                var discardPile = new DiscardPile(
                    new[] {Card(1, Clubs)}
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
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Spades)
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
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
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
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
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
                var playedCard = new Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(1, Spades)
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
                var playedCard = Card(8, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
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
            public void ReturnIsSuccessFalse_WhenSelectedSuit_AndPlayDoesNotMatch()
            {
                var selectedSuit = Hearts;
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Clubs)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithSelectedSuit(selectedSuit)
                    .Build(playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
            }

            [Fact]
            public void ReturnIsSuccessTrueForAnyCardWhenFirstTurnAndDiscardCardIs8()
            {
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
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
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(playedCard);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(7, Spades)
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
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    Card(2, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(1, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(Card(1, Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(Card(1, Clubs));
            }

            [Fact]
            public void AddPlayedEvent()
            {
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    Card(2, Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Diamonds)
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
                actualEvent.Cards.Should().Equal(playedCard);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    Card(2, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(1, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
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
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    Card(2, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
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
                var playedCard = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    playedCard
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
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
                var playedCard = Card(8, Clubs);
                var player1Hand = new CardCollection(
                    playedCard,
                    Card(1, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCard);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
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
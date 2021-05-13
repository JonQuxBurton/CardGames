using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.Suit;
using Action = SheddingCardGames.Domain.Action;

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

            public PlayCommand Build(ImmutableList<Card> playedCards)
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
                    CurrentTurn = new CurrentTurn(turnNumber, currentPlayer, Action.Play)
                };

                return new PlayCommand(new Rules(), gameState,  new PlayContext(executingPlayer, playedCards));
            }
        }

        public class IsValidShould
        {
            private readonly CardParser cardParser = new CardParser();

            [Fact]
            public void ReturnIsSuccessTrueWhenValid()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Clubs)
                ));
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenCardIsNotInPlayersHand()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection();
                var player2Hand = new CardCollection();
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Theory]
            [InlineData("2|Hearts", "2|Clubs", "2|Diamonds")]
            [InlineData("2|Clubs", "2|Hearts", "2|Diamonds")]
            [InlineData("2|Clubs", "2|Diamonds", "2|Hearts")]
            public void ReturnIsSuccessFalseWhenAnyCardIsNotInPlayersHand(params string[] playedCardsData)
            {
                var cardsPlayed = Cards(cardParser.Parse(playedCardsData));
                var player1Hand = new CardCollection(
                    Card(2, Clubs),
                    Card(2, Diamonds)
                    );
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(
                    new[] { Card(1, Clubs) }
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenNotPlayersTurn()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var discardPile = new DiscardPile(
                    new[] { Card(1, Clubs) }
                );
                var player1Hand = new CardCollection();
                var player2Hand = new CardCollection(
                    cardsPlayed
                );
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithExecutingPlayer(2)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnIsSuccessFalseForInvalidPlay()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }

            [Theory]
            [InlineData("2|Clubs", "2|Diamonds", "10|Hearts")]
            [InlineData("2|Clubs", "10|Hearts", "2|Diamonds")]
            [InlineData("10|Hearts", "2|Clubs", "2|Diamonds")]
            public void ReturnIsSuccessFalseForInvalidPlayWheMultipleCardsPlayed(params string[] playedCardsData)
            {
                var cardsPlayed = Cards(cardParser.Parse(playedCardsData));
                var player1Hand = new CardCollection(cardsPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingSuit()
            {
                var cardPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardPlayed);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingSuitWhenSelectedSuit()
            {
                var playedCards = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(playedCards);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(playedCards);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingRank()
            {
                var cardsPlayed = Cards(Card(1, Clubs), Card(1, Diamonds), Card(1, Hearts));
                var player1Hand = new CardCollection(cardsPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithRank8()
            {
                var cardsPlayed = Cards(Card(8, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForAnyCardWhenFirstTurnAndDiscardCardIs8()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenFirstTurnAndDiscardCardIsNot8()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var player2Hand = new CardCollection();
                var discardPile = new DiscardPile(new CardCollection(
                    Card(7, Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

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
                var cardsPlayed = Cards(
                    Card(1, Clubs), 
                    Card(1, Diamonds)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                ));
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var discardPile = new DiscardPile(new [] { Card(1, Hearts) });
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(Card(1, Diamonds));
                actual.CurrentTable.DiscardPile.RestOfCards.Cards.First().Should().Be(Card(1, Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(Card(1, Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(Card(1, Diamonds));
            }

            [Fact]
            public void AddPlayedEvent()
            {
                var cardsPlayed = Cards(
                    Card(1, Clubs),
                    Card(1, Diamonds)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                    ));
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var discardPile = new DiscardPile(new[] { Card(1, Hearts) });
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                actual.Events.Last().Should().BeOfType<Played>();
                var actualEvent = actual.Events.Last() as Played;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.PlayerNumber.Should().Be(1);
                actualEvent.Cards.Should().Equal(cardsPlayed);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var cardsPlayed = Cards
                (
                    Card(1, Clubs),
                    Card(1, Diamonds)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                    ));
                var player2Hand = new CardCollection(
                    Card(2, Diamonds),
                    Card(3, Diamonds)
                );
                var discardPile = new DiscardPile(new[] { Card(1, Hearts) });
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

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

                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(cardsPlayed);
                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardsPlayed.Last());
            }

            [Fact]
            public void CreateNewTurnWithNextActionTake()
            {
                var cardsPlayed = Cards (
                    Card(1, Clubs)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                    )
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

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
                var cardsPlayed = Cards(
                    Card(1, Clubs)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

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
                var cardsPlayed = Cards(Card(8, Clubs));
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                    Card(1, Clubs)
                    )
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

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

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardsPlayed.First());
            }
        }
    }
}
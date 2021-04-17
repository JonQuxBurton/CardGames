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
        public class ExecuteShould
        {
            [Fact]
            public void ReturnUpdatedTable()
            {
                var sampleData = new SampleData();
                var currentPlayer = sampleData.Player1;
                var table = TableCreator.Create(new StockPile(new CardCollection()), new DiscardPile(), currentPlayer,
                    sampleData.Player2);
                var context = new PlayCommandContext(currentPlayer, new Card(1, Suit.Clubs));
                var gameState = new GameState
                {
                    CurrentTable = table,
                    Events = new List<DomainEvent>(),
                    CurrentPlayer = currentPlayer,
                    CurrentTurn = new CurrentTurn(1, currentPlayer, new Card[0], false, null, Action.Play, null)
                };
                var sut = new PlayCommand(currentPlayer, new Rules(), gameState, context);

                var actual = sut.Execute();

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(new Card(1, Suit.Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(new Card(1, Suit.Clubs));
            }

            [Fact]
            public void AddPlayedEvent()
            {
                var expectedCard = new Card(1, Suit.Clubs);
                var sampleData = new SampleData();
                var currentPlayer = sampleData.Player1;
                currentPlayer.Hand = new CardCollection(
                    expectedCard,
                    new Card(2, Suit.Clubs)
                );
                var table = TableCreator.Create(new StockPile(new CardCollection()), new DiscardPile(), currentPlayer,
                    sampleData.Player2);
                var context = new PlayCommandContext(currentPlayer, expectedCard);
                var gameState = new GameState
                {
                    CurrentTable = table,
                    Events = new List<DomainEvent>(),
                    CurrentPlayer =  currentPlayer
                };
                var sut = new PlayCommand(currentPlayer, new Rules(), gameState, context);

                var actual = sut.Execute();

                actual.Events.Last().Should().BeOfType<Played>();
                var actualEvent = actual.Events.Last() as Played;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.PlayerNumber.Should().Be(currentPlayer.Number);
                actualEvent.Card.Should().Be(new Card(1, Suit.Clubs));
            }
        }

        public class IsValidShould
        {
            private PlayCommand CreateSut(DiscardPile discardPile, CardCollection player1Hand, Card playedCard,
                int startingPlayer = 1, Suit? selectedSuit = null, int turnNumber = 2)
            {
                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var currentPlayer = player1;
                if (startingPlayer == 2)
                    currentPlayer = sampleData.Player2;

                discardPile.TurnUpTopCard();
                var table = TableCreator.Create(new StockPile(new CardCollection()), discardPile, player1,
                    sampleData.Player2);
                var context = new PlayCommandContext(currentPlayer, playedCard);
                var gameState = new GameState
                {
                    CurrentTable = table,
                    Events = new List<DomainEvent>(),
                    SelectedSuit = selectedSuit,
                    TurnNumber = turnNumber
                };

                return new PlayCommand(player1, new Rules(), gameState, context);
            }

            [Fact]
            public void ReturnIsSuccessTrueWhenValid()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(2, Suit.Clubs)
                ));
                var sut = CreateSut(discardPile, player1Hand, playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenCardIsNotInPlayersHand()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var discardPile = new DiscardPile();
                var player1Hand = new CardCollection();

                var sut = CreateSut(discardPile, player1Hand, playedCard);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenNotPlayersTurn()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var discardPile = new DiscardPile();
                var player1Hand = new CardCollection();

                var sut = CreateSut(discardPile, player1Hand, playedCard, 2);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnIsSuccessFalseForInvalidPlay()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(2, Suit.Spades)
                ));
                var sut = CreateSut(discardPile, player1Hand, playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingSuit()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(1, Suit.Spades)
                ));

                var sut = CreateSut(discardPile, player1Hand, playedCard);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingSuitWhenSelectedSuit()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(8, Suit.Spades)
                ));

                var sut = CreateSut(discardPile, player1Hand, playedCard, 1, Suit.Clubs);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithMatchingRank()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(1, Suit.Spades)
                ));

                var sut = CreateSut(discardPile, player1Hand, playedCard);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForValidPlayWithRank8()
            {
                var playedCard = new Card(8, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(1, Suit.Spades)
                ));

                var sut = CreateSut(discardPile, player1Hand, playedCard);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessTrueForAnyCardWhenFirstTurnAndDiscardCardIs8()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(8, Suit.Spades)
                ));

                var sut = CreateSut(discardPile, player1Hand, playedCard, 1, null, 1);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsSuccessFalseWhenFirstTurnAndDiscardCardIsNot8()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);

                var discardPile = new DiscardPile(new CardCollection(
                    new Card(7, Suit.Spades)
                ));

                var sut = CreateSut(discardPile, player1Hand, playedCard, 1, null, 1);

                var actual = sut.IsValid();
                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }
        }
    }
}
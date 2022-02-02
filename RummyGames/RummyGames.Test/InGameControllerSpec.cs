using System;
using System.Linq;
using CardGamesDomain;
using FluentAssertions;
using Xunit;

namespace RummyGames.Test
{
    namespace InGameControllerSpec
    {
        public class ShuffleDeckShould
        {
            private readonly DeckBuilder deckBuilder;
            private readonly Player guest;
            private readonly Player host;

            public ShuffleDeckShould()
            {
                deckBuilder = new DeckBuilder();
                host = new Player(Guid.NewGuid(), "Alice");
                guest = new Player(Guid.NewGuid(), "Bob");
            }

            [Fact]
            public void ShuffleDeck()
            {
                var expected = deckBuilder.Build().Cards.Reverse().ToList();
                var inGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] {host, guest}, deckBuilder.Build(), null),
                    host.Id, null);
                var shuffler = new FakeShuffler(expected);
                var sut = new InGameController(shuffler);

                var actual = sut.ShuffleDeck(inGameState);

                actual.Table.Deck.Cards.Should().Equal(expected);
            }
        }

        public class DealShould
        {
            private readonly DeckBuilder deckBuilder;
            private readonly InGameState initialInGameState;

            public DealShould()
            {
                deckBuilder = new DeckBuilder();
                var host = new Player(Guid.NewGuid(), "Alice");
                var guest = new Player(Guid.NewGuid(), "Bob");
                initialInGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] {host, guest}, deckBuilder.Build(), null),
                    host.Id, null);
            }

            private InGameController CreateSut()
            {
                var expected = deckBuilder.Build().Cards.ToList();

                var shuffler = new FakeShuffler(expected);
                return new InGameController(shuffler);
            }

            [Fact]
            public void DealCardsToPlayer1()
            {
                var sut = CreateSut();

                var actual = sut.Deal(initialInGameState);

                actual.Table.Players.First().Hand.Cards.Should().Equal(
                    new Card(Rank.ACE, Suit.CLUBS),
                    new Card(Rank.ACE, Suit.HEARTS),
                    new Card(Rank.TWO, Suit.CLUBS),
                    new Card(Rank.TWO, Suit.HEARTS),
                    new Card(Rank.THREE, Suit.CLUBS),
                    new Card(Rank.THREE, Suit.HEARTS),
                    new Card(Rank.FOUR, Suit.CLUBS),
                    new Card(Rank.FOUR, Suit.HEARTS),
                    new Card(Rank.FIVE, Suit.CLUBS),
                    new Card(Rank.FIVE, Suit.HEARTS)
                );
            }

            [Fact]
            public void DealCardsToPlayer2()
            {
                var sut = CreateSut();

                var actual = sut.Deal(initialInGameState);

                actual.Table.Players.ElementAt(1).Hand.Cards.Should().Equal(
                    new Card(Rank.ACE, Suit.DIAMONDS),
                    new Card(Rank.ACE, Suit.SPADES),
                    new Card(Rank.TWO, Suit.DIAMONDS),
                    new Card(Rank.TWO, Suit.SPADES),
                    new Card(Rank.THREE, Suit.DIAMONDS),
                    new Card(Rank.THREE, Suit.SPADES),
                    new Card(Rank.FOUR, Suit.DIAMONDS),
                    new Card(Rank.FOUR, Suit.SPADES),
                    new Card(Rank.FIVE, Suit.DIAMONDS),
                    new Card(Rank.FIVE, Suit.SPADES)
                );
            }

            [Fact]
            public void DealCardToDiscardPile()
            {
                var sut = CreateSut();

                var actual = sut.Deal(initialInGameState);

                actual.Table.DiscardPile.TurnedUpCard.Should().Be(new Card(Rank.SIX, Suit.CLUBS));
            }

            [Fact]
            public void DealRestOfCardsToStockPile()
            {
                var sut = CreateSut();

                var actual = sut.Deal(initialInGameState);

                actual.Table.StockPile.Cards.ElementAt(0).Should().Be(new Card(Rank.SIX, Suit.DIAMONDS));
                actual.Table.StockPile.Cards.ElementAt(1).Should().Be(new Card(Rank.SIX, Suit.HEARTS));
                actual.Table.StockPile.Cards.ElementAt(2).Should().Be(new Card(Rank.SIX, Suit.SPADES));

                actual.Table.StockPile.Cards.Should().Equal(
                    new Card(Rank.SIX, Suit.DIAMONDS),
                    new Card(Rank.SIX, Suit.HEARTS),
                    new Card(Rank.SIX, Suit.SPADES),
                    new Card(Rank.SEVEN, Suit.CLUBS),
                    new Card(Rank.SEVEN, Suit.DIAMONDS),
                    new Card(Rank.SEVEN, Suit.HEARTS),
                    new Card(Rank.SEVEN, Suit.SPADES),
                    new Card(Rank.EIGHT, Suit.CLUBS),
                    new Card(Rank.EIGHT, Suit.DIAMONDS),
                    new Card(Rank.EIGHT, Suit.HEARTS),
                    new Card(Rank.EIGHT, Suit.SPADES),
                    new Card(Rank.NINE, Suit.CLUBS),
                    new Card(Rank.NINE, Suit.DIAMONDS),
                    new Card(Rank.NINE, Suit.HEARTS),
                    new Card(Rank.NINE, Suit.SPADES),
                    new Card(Rank.TEN, Suit.CLUBS),
                    new Card(Rank.TEN, Suit.DIAMONDS),
                    new Card(Rank.TEN, Suit.HEARTS),
                    new Card(Rank.TEN, Suit.SPADES),
                    new Card(Rank.JACK, Suit.CLUBS),
                    new Card(Rank.JACK, Suit.DIAMONDS),
                    new Card(Rank.JACK, Suit.HEARTS),
                    new Card(Rank.JACK, Suit.SPADES),
                    new Card(Rank.QUEEN, Suit.CLUBS),
                    new Card(Rank.QUEEN, Suit.DIAMONDS),
                    new Card(Rank.QUEEN, Suit.HEARTS),
                    new Card(Rank.QUEEN, Suit.SPADES),
                    new Card(Rank.KING, Suit.CLUBS),
                    new Card(Rank.KING, Suit.DIAMONDS),
                    new Card(Rank.KING, Suit.HEARTS),
                    new Card(Rank.KING, Suit.SPADES)
                );
            }

            [Fact]
            public void CreateTurn()
            {
                var sut = CreateSut();

                var actual = sut.Deal(initialInGameState);

                actual.CurrentTurn.Number.Should().Be(1);
                actual.CurrentTurn.CurrentPlayerId.Should().Be(initialInGameState.StartingPlayerId);
            }
        }

        public class TakeFromStockPileShould
        {
            private readonly DeckBuilder deckBuilder;
            private InGameState initialInGameState;
            private Player guest;
            private Player host;

            public TakeFromStockPileShould()
            {
                deckBuilder = new DeckBuilder();
                host = new Player(Guid.NewGuid(), "Alice");
                guest = new Player(Guid.NewGuid(), "Bob");
                initialInGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] { host, guest }, deckBuilder.Build(), null),
                    host.Id, null);
            }

            private InGameController CreateSut()
            {
                var expected = deckBuilder.Build().Cards.ToList();

                var shuffler = new FakeShuffler(expected);
                return new InGameController(shuffler);
            }

            [Fact]
            public void ReturnIsSuccessFalse_When_NotPlayerTurn()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromStockPile(currentInGameState, initialInGameState.Table.Players.ElementAt(1));

                actual.IsSuccess.Should().BeFalse();
                actual.ErrorKey.Should().Be(ErrorKey.NotTurn);
                actual.NewInGameState.CurrentTurn.TakenCard.Should().BeNull();
            }
            
            [Fact]
            public void ReturnIsSuccessFalse_When_PlayerAlreadyTaken()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var result = sut.TakeFromStockPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                var actual = sut.TakeFromStockPile(result.NewInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.IsSuccess.Should().BeFalse();
                actual.ErrorKey.Should().Be(ErrorKey.AlreadyTaken);
            }
            
            [Fact]
            public void AddTakenCardToCurrentTurn()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var expected = currentInGameState.Table.StockPile.TopCard;

                var actual = sut.TakeFromStockPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.NewInGameState.CurrentTurn.TakenCard.Should().Be(expected);
            }
            
            [Fact]
            public void AddCardToPlayer1()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromStockPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.NewInGameState.Table.Players.First().Hand.Cards.Should().Contain(new Card(Rank.SIX, Suit.DIAMONDS));
                actual.NewInGameState.CurrentTurn.Number.Should().Be(1);
                actual.NewInGameState.CurrentTurn.CurrentPlayerId.Should().Be(host.Id);
            }
            
            [Fact]
            public void RemoveCardFromStockPile()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromStockPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.NewInGameState.Table.StockPile.Cards.Should().NotContain(new Card(Rank.SIX, Suit.DIAMONDS));
            }

            [Fact]
            public void AddCardToPlayer2()
            {
                initialInGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] { host, guest }, deckBuilder.Build(), null),
                    guest.Id, null);

                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromStockPile(currentInGameState, initialInGameState.Table.Players.ElementAt(1));

                actual.NewInGameState.Table.Players.ElementAt(1).Hand.Cards.Should().Contain(new Card(Rank.SIX, Suit.DIAMONDS));
            }

        }
        
        public class TakeFromDiscardPileShould
        {
            private readonly DeckBuilder deckBuilder;
            private InGameState initialInGameState;
            private Player guest;
            private Player host;

            public TakeFromDiscardPileShould()
            {
                deckBuilder = new DeckBuilder();
                host = new Player(Guid.NewGuid(), "Alice");
                guest = new Player(Guid.NewGuid(), "Bob");
                initialInGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] { host, guest }, deckBuilder.Build(), null),
                    host.Id, null);
            }

            private InGameController CreateSut()
            {
                var expected = deckBuilder.Build().Cards.ToList();

                var shuffler = new FakeShuffler(expected);
                return new InGameController(shuffler);
            }

            [Fact]
            public void ReturnIsSuccessFalse_When_NotPlayerTurn()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromDiscardPile(currentInGameState, initialInGameState.Table.Players.ElementAt(1));

                actual.IsSuccess.Should().BeFalse();
                actual.ErrorKey.Should().Be(ErrorKey.NotTurn);
                actual.NewInGameState.CurrentTurn.TakenCard.Should().BeNull();
            }

            [Fact]
            public void ReturnIsSuccessFalse_When_PlayerAlreadyTaken()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var result = sut.TakeFromStockPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                var actual = sut.TakeFromDiscardPile(result.NewInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.IsSuccess.Should().BeFalse();
                actual.ErrorKey.Should().Be(ErrorKey.AlreadyTaken);
            }

            [Fact]
            public void AddTakenCardToCurrentTurn()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var expected = currentInGameState.Table.DiscardPile.TurnedUpCard;

                var actual = sut.TakeFromDiscardPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.NewInGameState.CurrentTurn.TakenCard.Should().Be(expected);
            }

            [Fact]
            public void AddCardToPlayer1()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromDiscardPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.NewInGameState.Table.Players.First().Hand.Cards.Should().Contain(new Card(Rank.SIX, Suit.CLUBS));
                actual.NewInGameState.CurrentTurn.Number.Should().Be(1);
                actual.NewInGameState.CurrentTurn.CurrentPlayerId.Should().Be(host.Id);
            }

            [Fact]
            public void RemoveCardFromDiscardPile()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromDiscardPile(currentInGameState, initialInGameState.Table.Players.ElementAt(0));

                actual.NewInGameState.Table.DiscardPile.Cards.Should().NotContain(new Card(Rank.SIX, Suit.CLUBS));
            }

            [Fact]
            public void AddCardToPlayer2()
            {
                initialInGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] { host, guest }, deckBuilder.Build(), null),
                    guest.Id, null);

                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);

                var actual = sut.TakeFromDiscardPile(currentInGameState, initialInGameState.Table.Players.ElementAt(1));

                actual.NewInGameState.Table.Players.ElementAt(1).Hand.Cards.Should().Contain(new Card(Rank.SIX, Suit.CLUBS));
            }

        }
        
        public class DiscardShould
        {
            private readonly DeckBuilder deckBuilder;
            private InGameState initialInGameState;
            private readonly Player guest;
            private readonly Player host;

            public DiscardShould()
            {
                deckBuilder = new DeckBuilder();
                host = new Player(Guid.NewGuid(), "Alice");
                guest = new Player(Guid.NewGuid(), "Bob");
                initialInGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] { host, guest }, deckBuilder.Build(), null),
                    host.Id, null);
            }

            private InGameController CreateSut()
            {
                var expected = deckBuilder.Build().Cards.ToList();

                var shuffler = new FakeShuffler(expected);
                return new InGameController(shuffler);
            }

            [Fact]
            public void ReturnIsSuccessFalse_When_NotPlayerTurn()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var discardCard = new Card(Rank.FIVE, Suit.HEARTS);

                var actual = sut.Discard(currentInGameState, currentInGameState.Table.Players.ElementAt(1), discardCard);

                actual.IsSuccess.Should().BeFalse();
                actual.ErrorKey.Should().Be(ErrorKey.NotTurn);
            }

            [Fact]
            public void ReturnIsSuccessFalse_When_PlayerHasNotTaken()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var discardCard = new Card(Rank.FIVE, Suit.HEARTS);

                var actual = sut.Discard(currentInGameState, currentInGameState.Table.Players.ElementAt(0), discardCard);

                actual.IsSuccess.Should().BeFalse();
                actual.ErrorKey.Should().Be(ErrorKey.InvalidAction);
            }

            [Fact]
            public void RemoveCardFromPlayer1()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var result = sut.TakeFromStockPile(currentInGameState, currentInGameState.Table.Players.ElementAt(0));
                currentInGameState = result.NewInGameState;
                var discardCard = new Card(Rank.FIVE, Suit.HEARTS);

                var actual = sut.Discard(currentInGameState, currentInGameState.Table.Players.ElementAt(0), discardCard);

                actual.NewInGameState.Table.Players.First().Hand.Cards.Should().NotContain(discardCard);
                actual.NewInGameState.CurrentTurn.Number.Should().Be(2);
                actual.NewInGameState.CurrentTurn.CurrentPlayerId.Should().Be(guest.Id);
            }

            [Fact]
            public void AddCardToDiscardPile()
            {
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var result = sut.TakeFromStockPile(currentInGameState, currentInGameState.Table.Players.ElementAt(0));
                currentInGameState = result.NewInGameState;
                var discardCard = new Card(Rank.FIVE, Suit.HEARTS);

                var actual = sut.Discard(currentInGameState, currentInGameState.Table.Players.ElementAt(0), discardCard);

                actual.NewInGameState.Table.DiscardPile.Cards.Should().Contain(discardCard);
            }

            [Fact]
            public void RemoveCardFromPlayer2()
            {
                initialInGameState = new InGameState(Guid.NewGuid(),
                    new Table(new[] { host, guest }, deckBuilder.Build(), null),
                    guest.Id, null);
                var discardCard = new Card(Rank.FIVE, Suit.SPADES);
                var sut = CreateSut();
                var currentInGameState = sut.Deal(initialInGameState);
                var result = sut.TakeFromStockPile(currentInGameState, currentInGameState.Table.Players.ElementAt(1));
                currentInGameState = result.NewInGameState;

                var actual = sut.Discard(currentInGameState, currentInGameState.Table.Players.ElementAt(1), discardCard);

                actual.NewInGameState.Table.Players.ElementAt(1).Hand.Cards.Should().NotContain(discardCard);
                actual.NewInGameState.CurrentTurn.Number.Should().Be(2);
                actual.NewInGameState.CurrentTurn.CurrentPlayerId.Should().Be(host.Id);
            }

        }
    }
}
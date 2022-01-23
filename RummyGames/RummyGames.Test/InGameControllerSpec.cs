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
                    host);
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
                    host);
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
        }
    }
}
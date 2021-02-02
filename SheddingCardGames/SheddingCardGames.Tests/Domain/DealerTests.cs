using System.Linq;
using FluentAssertions;
using Moq;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace DealerTests
    {
        public class BuildShould
        {
            private readonly CardCollection deck;
            private readonly Card[] expectedCards;
            private readonly Rules rules;
            private readonly Mock<IShuffler> shufflerMock;
            private readonly Dealer sut;
            private readonly Player player1;
            private readonly Player player2;

            public BuildShould()
            {
                expectedCards = new[]
                {
                    new Card(1, Suit.Diamonds),
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Diamonds),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Diamonds),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Diamonds),
                    new Card(4, Suit.Spades),
                    new Card(5, Suit.Diamonds),
                    new Card(5, Suit.Spades),
                    new Card(6, Suit.Diamonds),
                    new Card(6, Suit.Spades),
                    new Card(7, Suit.Diamonds),
                    new Card(7, Suit.Spades),

                    new Card(13, Suit.Hearts),

                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                };
                deck = new CardCollectionBuilder().Build();
                shufflerMock = new Mock<IShuffler>();
                shufflerMock.Setup(x => x.Shuffle(deck.Cards)).Returns(expectedCards);
                rules = new Rules();
                sut = new Dealer(rules, shufflerMock.Object, deck);
                player1 = new Player(1);
                player2 = new Player(2);
            }

            [Fact]
            public void ShuffleDeck()
            {
                sut.Build(new[] {player1, player2});

                shufflerMock.Verify(x => x.Shuffle(deck.Cards));
            }

            [Fact]
            public void DealCardsToPlayer1()
            {
                var actual = sut.Build(new[] {player1, player2});

                actual.Player1.Hand.Cards.Should().Equal(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds),
                    new Card(6, Suit.Diamonds),
                    new Card(7, Suit.Diamonds)
                );
            }

            [Fact]
            public void DealCardsToPlayer2()
            {
                var actual = sut.Build(new[] {player1, player2});

                actual.Player2.Hand.Cards.Should().Equal(
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Spades),
                    new Card(5, Suit.Spades),
                    new Card(6, Suit.Spades),
                    new Card(7, Suit.Spades)
                );
            }

            [Fact]
            public void MoveCardToDiscardPile()
            {
                var actual = sut.Build(new[] {player1, player2});

                actual.StockPile.Cards.Cards.Count().Should().Be(expectedCards.Length - rules.GetHandSize() * 2 - 1);
                actual.DiscardPile.CardToMatch.Should().Be(new Card(13, Suit.Hearts));
            }
        }
    }
}
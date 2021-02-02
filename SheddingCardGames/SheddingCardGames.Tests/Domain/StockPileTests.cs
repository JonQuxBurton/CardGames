using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace StockPileTests
    {
        public class ConstructorShould
        {
            [Fact]
            public void SetCards()
            {
                var expectedCards = new CardCollection(new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                });

                var sut = new StockPile(expectedCards);

                sut.Cards.Cards.Should().Equal(expectedCards.Cards);
            }
        }

        public class TakeShould
        {
            private readonly StockPile sut;

            public TakeShould()
            {
                var cards = new CardCollection(new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                });

                sut = new StockPile(cards);
            }

            [Fact]
            public void ReturnTakenCard()
            {
                var actual = sut.Take();

                actual.Should().Be(new Card(1, Suit.Clubs));
            }

            [Fact]
            public void RemoveTakenCard()
            {
                sut.Take();

                sut.Cards.Cards.Should().Equal(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                );
            }
        }

        public class IsEmptyShould
        {
            [Fact]
            public void ReturnTrueWhenContainsNoCards()
            {
                var sut = new StockPile(new CardCollection());

                var actual = sut.IsEmpty();

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalseWhenContainsCards()
            {
                var cards = new CardCollection(new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                });
                var sut = new StockPile(cards);

                var actual = sut.IsEmpty();

                actual.Should().BeFalse();
            }
        }

        public class AddAtEndShould
        {
            private readonly StockPile sut;

            public AddAtEndShould()
            {
                var cards = new CardCollection(new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                });

                sut = new StockPile(cards);
            }

            [Fact]
            public void AddCardAtEnd()
            {
                var addedCard = new Card(13, Suit.Diamonds);

                sut.AddAtEnd(addedCard);

                sut.Cards.Cards.Last().Should().Be(addedCard);
            }
        }
    }
}
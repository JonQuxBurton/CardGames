using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace CardCollectionTests
    {
        public class GetCardShould
        {
            private readonly CardCollection sut;

            public GetCardShould()
            {
                sut = new CardCollection(new[]
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });
            }
            
            [Fact]
            public void ReturnCard()
            {
                var actual = sut.Get(2, Suit.Diamonds);
                
                actual.Should().Be(new Card(2, Suit.Diamonds));
            }
            
            [Fact]
            public void ReturnNullWhenCardNotFound()
            {
                var actual = sut.Get(13, Suit.Clubs);

                actual.Should().BeNull();
            }
        }
        
        public class GetCardCollectionShould
        {
            private readonly CardCollection sut;

            public GetCardCollectionShould()
            {
                sut = new CardCollection(new[]
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });
            }
            
            [Fact]
            public void ReturnCards()
            {
                var actual = sut.GetCardCollection(new Card(1, Suit.Diamonds), new Card(3, Suit.Diamonds));
                
                actual.Cards.Should().Equal(
                    new Card(1, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                );
            }
            
            [Fact]
            public void ReturnEmptyWhenCardsNotFound()
            {
                var actual = sut.GetCardCollection(new Card(13, Suit.Clubs));

                actual.Cards.Should().BeEmpty();
            }
        }
        
        public class AddAtStartShould
        {
            [Fact]
            public void AddCardAtStart()
            {
                var sut = new CardCollection(new []
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });
                
                sut.AddAtStart(new Card(13, Suit.Clubs));

                sut.Cards.First().Should().Be(new Card(13, Suit.Clubs));
            }
            
            [Fact]
            public void AddCardAtStartWhenEmpty()
            {
                var sut = new CardCollection();
                
                sut.AddAtStart(new Card(13, Suit.Clubs));

                sut.Cards.Should().Equal(new Card(13, Suit.Clubs));
            }
        }
        
        public class AddAtEndShould
        {
            [Fact]
            public void AddCardAtEnd()
            {
                var sut = new CardCollection(new []
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });
                
                sut.AddAtEnd(new Card(13, Suit.Clubs));

                sut.Cards.Last().Should().Be(new Card(13, Suit.Clubs));
            }
            
            [Fact]
            public void AddCardAtEndWhenEmpty()
            {
                var sut = new CardCollection();
                
                sut.AddAtEnd(new Card(13, Suit.Clubs));

                sut.Cards.Should().Equal(new Card(13, Suit.Clubs));
            }
        }

        public class TakeFromStartShould
        {
            [Fact]
            public void ReturnCardFromStart()
            {
                var sut = new CardCollection(new[]
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });

                var actual = sut.TakeFromStart();

                actual.Should().Be(new Card(1, Suit.Diamonds));
                sut.Cards.Should().Equal(
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                    );
            }

            [Fact]
            public void ReturnNullWhenEmpty()
            {
                var sut = new CardCollection();

                var actual = sut.TakeFromStart();

                actual.Should().BeNull();
            }
        }
        
        public class RemoveShould
        {
            [Fact]
            public void RemoveCard()
            {
                var sut = new CardCollection(new[]
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });

                sut.Remove(new Card(2, Suit.Diamonds));

                sut.Cards.Should().Equal(
                    new Card(1, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                    );
            }
        }

        public class ContainsShould
        {
            private readonly CardCollection sut;

            public ContainsShould()
            {
                sut = new CardCollection(new[]
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });
            }

            [Fact]
            public void ReturnTrueWhenCollectionContainsCard()
            {
                var actual = sut.Contains(new Card(2, Suit.Diamonds));

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalseWhenCollectionDoesNotContainCard()
            {
                var actual = sut.Contains(new Card(13, Suit.Clubs));

                actual.Should().BeFalse();
            }
        }
        
        public class ContainsAllShould
        {
            private readonly CardCollection sut;

            public ContainsAllShould()
            {
                sut = new CardCollection(
                    new Card(1, Suit.Diamonds), 
                    new Card(2, Suit.Diamonds), 
                    new Card(3, Suit.Diamonds));
            }

            [Fact]
            public void ReturnTrueWhenCollectionContainsAllCardsToCheck()
            {
                var actual = sut.ContainsAll(ImmutableList.Create(new Card(1, Suit.Diamonds), new Card(2, Suit.Diamonds)));

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalseWhenCollectionDoesNotContainsAllCardsToCheck()
            {
                var actual = sut.ContainsAll(ImmutableList.Create(new Card(1, Suit.Diamonds), new Card(1, Suit.Clubs)));

                actual.Should().BeFalse();
            }
            
            [Fact]
            public void ReturnFalseWhenCardsToCheckIsEmpty()
            {
                var actual = sut.ContainsAll(ImmutableList<Card>.Empty);

                actual.Should().BeFalse();
            }
            
            [Fact]
            public void ReturnFalseWhenCardsToCheckIsNull()
            {
                var actual = sut.ContainsAll(null);

                actual.Should().BeFalse();
            }
        }
        
        public class IsEmpty
        {
            [Fact]
            public void ReturnTrueWhenEmpty()
            {
                var sut = new CardCollection();
                
                var actual = sut.IsEmpty();

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalseWhenNonEmpty()
            {
                var sut = new CardCollection(new[]
                {
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                });

                var actual = sut.IsEmpty();

                actual.Should().BeFalse();
            }
        }

    }
}

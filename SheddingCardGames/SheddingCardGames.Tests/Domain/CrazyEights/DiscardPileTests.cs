using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using Xunit;

namespace SheddingCardGames.Tests.Domain.CrazyEights
{
    namespace DiscardPileTests
    {
        public class ConstructorShould
        {
            private readonly Card[] expectedCards;
            private DiscardPile sut;

            public ConstructorShould()
            {
                expectedCards = new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                };
                
            }
            
            [Fact]
            public void SetRestOfCards()
            {
                sut = new DiscardPile(expectedCards);
                sut.RestOfCards.Cards.Should().Equal(expectedCards);
            }

            [Fact]
            public void SetCardToMatchToNull()
            {
                sut = new DiscardPile(expectedCards);
                sut.CardToMatch.Should().BeNull();
            }

            [Fact]
            public void SetRestOfCardsToEmptyWhenNoCardSupplied()
            {
                sut = new DiscardPile();
                sut.RestOfCards.Cards.Should().BeEmpty();
            }
        }

        public class AllCardsShould
        {
            private DiscardPile sut;

            [Fact]
            public void ReturnEmptyWhenNoCards()
            {
                sut = new DiscardPile();
                
                var actual = sut.AllCards;

                actual.Cards.Should().BeEmpty();
            }
            
            [Fact]
            public void ReturnCardsWhenNoCardToMatch()
            {
                var expectedCards = new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                };
                sut = new DiscardPile(expectedCards);
                
                var actual = sut.AllCards;

                actual.Cards.Should().Equal(expectedCards);
            }
            
            [Fact]
            public void ReturnCardsWhenCardToMatch()
            {
                var expectedCards = new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                };
                sut = new DiscardPile(expectedCards);
                sut.TurnUpTopCard();
                
                var actual = sut.AllCards;

                actual.Cards.Should().Equal(expectedCards);
            }
        }
        
        public class TurnUpTopCardShould
        {
            private readonly Card[] cards;
            private DiscardPile sut;

            public TurnUpTopCardShould()
            {
                cards = new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                };

            }
            
            [Fact]
            public void SetCardToMatch()
            {
                sut = new DiscardPile(cards);
                
                sut.TurnUpTopCard();

                sut.CardToMatch.Should().Be(cards.First());
            }
            
            [Fact]
            public void TakeTopCardFromRestOfCards()
            {
                sut = new DiscardPile(cards);
                
                sut.TurnUpTopCard();

                sut.RestOfCards.Cards.Should().Equal(cards[1], cards[2]);
            }
            
        }
        
        public class AddCardShould
        {
            private readonly Card[] cards;
            private DiscardPile sut;
            private readonly Card discardedCard;

            public AddCardShould()
            {
                discardedCard = new Card(13, Suit.Diamonds);
                cards = new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                };
                sut = new DiscardPile(cards);
                sut.TurnUpTopCard();
            }
            
            [Fact]
            public void SetCardToMatch()
            {
                sut.AddCard(discardedCard);

                sut.CardToMatch.Should().Be(discardedCard);
            }
            
            [Fact]
            public void AddCurrentCardToMatchToStartOfRestOfCards()
            {
                sut.AddCard(discardedCard);

                sut.RestOfCards.Cards.Should().Equal(cards);
            }
            
            [Fact]
            public void NotErrorWhenNoCardToMatch()
            {
                sut = new DiscardPile(cards);
                sut.AddCard(discardedCard);

                sut.CardToMatch.Should().Be(discardedCard);
                sut.RestOfCards.Cards.Should().Equal(cards);
            }
        }
        
        public class TakeRestOfCardsShould
        {
            private readonly DiscardPile sut;

            public TakeRestOfCardsShould()
            {
                sut = new DiscardPile(new[]
                {
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                });
                sut.TurnUpTopCard();
            }
            
            [Fact]
            public void ReturnRestOfCards()
            {
                var actual = sut.TakeRestOfCards();

                actual.Cards.Should().Equal(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                );
            }
            
            [Fact]
            public void TakeRestOfCards()
            {
                sut.TakeRestOfCards();

                sut.RestOfCards.Cards.Should().BeEmpty();
            }
        }
    }
}

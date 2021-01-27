using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace RulesTests
    {
        public class GetValidPlaysShould
        {
            private readonly CardCollection deck;
            private readonly Rules sut;

            public GetValidPlaysShould()
            {
                deck = new CardCollectionBuilder().Build();
                sut = new Rules();
            }

            [Fact]
            public void ReturnNoPotentialPlays()
            {
                var discard = deck.Get(1, Suit.Clubs);
                var hand = deck.GetCardCollection(new Card(2, Suit.Hearts));

                var actual = sut.GetValidPlays(discard, hand, 2, null);

                actual.Should().BeEmpty();
            }

            [Fact]
            public void ReturnPotentialPlaysWithMatchingSuit()
            {
                var discard = deck.Get(1, Suit.Clubs);
                var hand = deck.GetCardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(2, Suit.Diamonds),
                    new Card(2, Suit.Hearts),
                    new Card(2, Suit.Spades));

                var actual = sut.GetValidPlays(discard, hand, 2, null);

                actual.Should().BeEquivalentTo(
                    deck.Get(2, Suit.Clubs),
                    deck.Get(3, Suit.Clubs),
                    deck.Get(4, Suit.Clubs),
                    deck.Get(5, Suit.Clubs));
            }

            [Fact]
            public void ReturnPotentialPlaysWithMatchingRank()
            {
                var discard = deck.Get(1, Suit.Clubs);
                var hand = deck.GetCardCollection(
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds),
                    new Card(1, Suit.Diamonds),
                    new Card(1, Suit.Hearts),
                    new Card(1, Suit.Spades));

                var actual = sut.GetValidPlays(discard, hand, 2, null);

                actual.Should().BeEquivalentTo(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(1, Suit.Hearts),
                    deck.Get(1, Suit.Spades)
                    );
            }

            [Fact]
            public void ReturnPotentialPlaysWithMatchingEight()
            {
                var discard = deck.Get(1, Suit.Clubs);
                var hand = deck.GetCardCollection(
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds),
                    new Card(6, Suit.Diamonds),
                    new Card(7, Suit.Diamonds),
                    new Card(8, Suit.Diamonds)
                    );
                var actual = sut.GetValidPlays(discard, hand, 2, null);

                actual.Should().BeEquivalentTo(
                    deck.Get(8, Suit.Diamonds));
            }
            
            [Fact]
            public void ReturnPotentialPlaysWithoutDuplicateEightWhenSuitAlsoMatches()
            {
                var discard = deck.Get(2, Suit.Diamonds);
                var hand = deck.GetCardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(8, Suit.Diamonds)
                    );
                var actual = sut.GetValidPlays(discard, hand, 2, null);

                actual.Should().BeEquivalentTo(
                    deck.Get(2, Suit.Clubs),
                    deck.Get(8, Suit.Diamonds)
                    );
            }

            [Fact]
            public void ReturnPotentialPlaysWithoutDuplicateEightWhenRankAlsoMatches()
            {
                var discard = deck.Get(8, Suit.Hearts);
                var hand = deck.GetCardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(8, Suit.Diamonds)
                );
                var actual = sut.GetValidPlays(discard, hand, 2, null);

                actual.Should().BeEquivalentTo(
                    deck.Get(8, Suit.Diamonds)
                );
            }

            [Fact]
            public void ReturnPotentialPlaysWhenFirstTurnAndDiscardCardIs8()
            {
                var discard = deck.Get(8, Suit.Hearts);
                var hand = deck.GetCardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(8, Suit.Diamonds)
                    );
                var actual = sut.GetValidPlays(discard, hand, 1, null);

                actual.Should().Equal(
                    hand.Cards
                );
            }

            [Fact]
            public void ReturnPotentialPlaysWithMatchingSuitWhenSelectedSuit()
            {
                var discard = deck.Get(8, Suit.Hearts);
                var hand = deck.GetCardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(2, Suit.Diamonds),
                    new Card(2, Suit.Hearts),
                    new Card(2, Suit.Spades));

                var actual = sut.GetValidPlays(discard, hand, 2, Suit.Clubs);

                actual.Should().BeEquivalentTo(
                    deck.Get(2, Suit.Clubs),
                    deck.Get(3, Suit.Clubs),
                    deck.Get(4, Suit.Clubs),
                    deck.Get(5, Suit.Clubs));
            }

        }

        public class IsValidPlayShould
        {
            private readonly CardCollection deck;

            public IsValidPlayShould()
            {
                deck = new CardCollectionBuilder().Build();
            }

            [Fact]
            public void ReturnFalseForInvalidPlay()
            {
                var discardCard = deck.Get(1, Suit.Clubs);
                var sut = new Rules();

                var actual = sut.IsValidPlay(deck.Get(10, Suit.Hearts), discardCard, 2, null);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrueForValidPlayWithMatchingSuit()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();
                
                var actual = sut.IsValidPlay(playedCard, discardCard, 2, null);

                actual.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnTrueForValidPlayWithMatchingSuitWhenSelectedSuit()
            {
                var discardCard = deck.Get(8, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Hearts);

                var sut = new Rules();
                
                var actual = sut.IsValidPlay(playedCard, discardCard, 2, Suit.Hearts);

                actual.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnFalseWhenMatchingSuitButSelectedSuit()
            {
                var discardCard = deck.Get(8, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();
                
                var actual = sut.IsValidPlay(playedCard, discardCard, 2, Suit.Hearts);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrueForValidPlayWithMatchingRank()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(10, Suit.Hearts);

                var sut = new Rules();
                
                var actual = sut.IsValidPlay(playedCard, discardCard, 2, null);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrueForValidPlayWithRank8()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var playedCard = deck.Get(8, Suit.Clubs);

                var sut = new Rules();
                
                var actual = sut.IsValidPlay(playedCard, discardCard, 2, null);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrueForAnyCardWhenFirstTurnAndDiscardCardIs8()
            {
                var discardCard = deck.Get(8, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();
                
                var actual = sut.IsValidPlay(playedCard, discardCard, 1, null);

                actual.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnFalseWhenFirstTurnAndDiscardCardIsNot8()
            {
                var discardCard = deck.Get(7, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();
                
                var actual = sut.IsValidPlay(playedCard, discardCard, 1, null);

                actual.Should().BeFalse();
            }
        }

    }
}

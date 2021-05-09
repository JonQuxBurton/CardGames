using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;

namespace SheddingCardGames.Tests.Domain
{
    namespace RulesTests
    {
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

                var actual = sut.IsValidPlay(Cards(deck.Get(10, Suit.Hearts)), discardCard, 2, null);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrueForValidPlayWithMatchingSuit()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, 2, null);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrueForValidPlayWithMatchingSuitWhenSelectedSuit()
            {
                var discardCard = deck.Get(8, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Hearts);

                var sut = new Rules();

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, 2, Suit.Hearts);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalseWhenMatchingSuitButSelectedSuit()
            {
                var discardCard = deck.Get(8, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, 2, Suit.Hearts);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrueForValidPlayWithMatchingRank()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(10, Suit.Hearts);

                var sut = new Rules();

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, 2, null);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrueForValidPlayWithRank8()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var playedCard = deck.Get(8, Suit.Clubs);

                var sut = new Rules();

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, 2, null);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrueForAnyCardWhenFirstTurnAndDiscardCardIs8()
            {
                var discardCard = deck.Get(8, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, 1, null);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalseWhenFirstTurnAndDiscardCardIsNot8()
            {
                var discardCard = deck.Get(7, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);

                var sut = new Rules();

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, 1, null);

                actual.Should().BeFalse();
            }
        }
    }
}
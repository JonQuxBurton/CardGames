using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain
{
    namespace RulesTests
    {
        public class IsValidPlayShould
        {
            private readonly Rules sut;

            public IsValidPlayShould()
            {
                sut = new Rules();
            }

            [Fact]
            public void ReturnFalse_WhenCardPlayed_AndCardIsInvalid()
            {
                var discardCard = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(Card(10, Hearts)), discardCard, null, true);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_WithMatchingSuit()
            {
                var discardCard = Card(10, Clubs);
                var playedCard = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_WithMatchingSuit_WhenSelectedSuit()
            {
                var discardCard = Card(8, Clubs);
                var playedCard = Card(1, Hearts);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, Hearts, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalse_WhenCardPlayed_WithMatchingSuit_ButNotSelectedSuit()
            {
                var discardCard = Card(8, Clubs);
                var playedCard = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, Hearts, true);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_WithMatchingRank()
            {
                var discardCard = Card(10, Clubs);
                var playedCard = Card(10, Hearts);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_WithRankEight()
            {
                var discardCard = Card(10, Hearts);
                var playedCard = Card(8, Clubs);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_ForAnyCard_WhenInitialDiscardCardsIsEight()
            {
                var discardCard = Card(8, Hearts);
                var playedCard = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, null, false);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenFirstTurn_AndEightPlayed_AndSelectedSuit()
            {
                var discardCard = Card(8, Hearts);
                var playedCard = Card(1, Clubs);
                var selectedSuit = Clubs;

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, selectedSuit, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnFalse_WhenFirstTurn_AndEightPlayed_AndNoSelectedSuit()
            {
                var discardCard = Card(8, Hearts);
                var playedCard = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, null, true);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnFalse_WhenInitialDiscardIsNotEight()
            {
                var discardCard = Card(7, Hearts);
                var playedCard = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(playedCard), discardCard, null, false);

                actual.Should().BeFalse();
            }
        }
    }
}
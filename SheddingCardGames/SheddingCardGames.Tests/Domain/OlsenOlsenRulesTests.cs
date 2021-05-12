using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;
using static SheddingCardGames.Domain.Suit;
using static SheddingCardGames.Domain.CardsUtils;

namespace SheddingCardGames.Tests.Domain
{
    namespace OlsenOlsenRulesTests
    {
        public class IsValidPlayShould
        {
            private readonly CardParser cardParser = new CardParser();
            private readonly OlsenOlsenRules sut;

            public IsValidPlayShould()
            {
                sut = new OlsenOlsenRules();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_MatchingSuit()
            {
                var discardCard = Card(10, Clubs);
                var cardPlayed = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(cardPlayed), discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_WithMatchingRank()
            {
                var discardCard = Card(10, Clubs);
                var cardPlayed = Card(10, Hearts);

                var actual = sut.IsValidPlay(Cards(cardPlayed), discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("10|Hearts", "10|Diamonds")]
            [InlineData("10|Hearts", "10|Diamonds", "10|Spades")]
            public void ReturnTrue_WhenMultipleCardsPlayed_WithMatchingRank(params string[] cardsPlayedData)
            {
                var discardCard = Card(10, Clubs);
                var cardsPlayed = Cards(cardParser.Parse(cardsPlayedData));

                var actual = sut.IsValidPlay(cardsPlayed, discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_WithRank8()
            {
                var discardCard = Card(10, Hearts);
                var cardPlayed = Card(8, Clubs);

                var actual = sut.IsValidPlay(Cards(cardPlayed), discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("8|Clubs", "8|Diamonds")]
            [InlineData("8|Clubs", "8|Diamonds", "8|Spades")]
            public void ReturnTrue_WhenMultipleCardsPlayed_WithRank8(params string[] cardsPlayedData)
            {
                var discardCard = Card(10, Hearts);
                var cardsPlayed = Cards(cardParser.Parse(cardsPlayedData));

                var actual = sut.IsValidPlay(cardsPlayed, discardCard, null, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenCardPlayed_WithMatchingSuit_AndSelectedSuit()
            {
                var discardCard = Card(8, Clubs);
                var cardPlayed = Card(1, Hearts);

                var actual = sut.IsValidPlay(Cards(cardPlayed), discardCard, Hearts, true);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_ForAnyCard_WhenInitialDiscardCardsIsEight()
            {
                var discardCard = Card(8, Hearts);
                var cardPlayed = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(cardPlayed), discardCard, null, false);

                actual.Should().BeTrue();
            }

            [Theory]
            [InlineData("1|Clubs", "1|Diamonds")]
            public void ReturnTrue_ForAnyCards_WhenInitialDiscardCardsIsEight(params string[] cardsPlayedData)
            {
                var discardCard = Card(8, Hearts);
                var cardsPlayed = Cards(cardParser.Parse(cardsPlayedData));

                var actual = sut.IsValidPlay(cardsPlayed, discardCard, null, false);

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

            [Fact]
            public void ReturnFalse_WhenCardPlayed_AndCardIsInvalid()
            {
                var discardCard = Card(1, Clubs);
                var cardPlayed = Card(10, Hearts);

                var actual = sut.IsValidPlay(Cards(cardPlayed), discardCard, null, true);

                actual.Should().BeFalse();
            }

            [Theory]
            [InlineData("2|Clubs", "10|Hearts")]
            [InlineData("2|Clubs", "3|Clubs", "10|Hearts")]
            public void ReturnFalse_WhenMultipleCardsPlayed_AndAnyCardIsInvalid(params string[] cardsPlayedData)
            {
                var discardCard = Card(1, Clubs);
                var cardsPlayed = Cards(cardParser.Parse(cardsPlayedData));

                var actual = sut.IsValidPlay(cardsPlayed, discardCard, null, true);

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnFalse_WhenCardPlayed_WithMatchingSuit_ButNotMatchingSelectedSuit()
            {
                var discardCard = Card(8, Clubs);
                var cardPlayed = Card(1, Clubs);

                var actual = sut.IsValidPlay(Cards(cardPlayed), discardCard, Hearts, true);

                actual.Should().BeFalse();
            }
        }
    }
}
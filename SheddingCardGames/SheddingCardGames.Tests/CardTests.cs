using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace SheddingCardGames.Tests
{
    namespace CardTests
    {
        public class ToStringShould
        {
            [Theory]
            [InlineData("Clubs", Suit.Clubs)]
            [InlineData("Diamonds", Suit.Diamonds)]
            [InlineData("Hearts", Suit.Hearts)]
            [InlineData("Spades", Suit.Spades)]
            public void ReturnCardsAsStrings(string suitName, Suit suit)
            {
                var cards = new List<Card>();
                for (var i = 0; i < 13; i++) 
                    cards.Add(new Card(i + 1, suit));

                var cardsAsStrings = cards.Select(x => x.ToString()).ToArray();

                for (int i = 0; i < 13; i++)
                {
                    var actual = cardsAsStrings[i];
                    actual.Should().Be($"{i+1} {suitName}");
                }
            }
        }
    }
}

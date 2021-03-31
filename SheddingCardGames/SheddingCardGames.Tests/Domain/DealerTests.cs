using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace DealerTests
    {
        public class DealShould
        {
            private readonly CardCollection deck;
            private readonly Rules rules;
            private readonly Dealer sut;
            private readonly Player player1;
            private readonly Player player2;
            private readonly Player player3;
            private readonly int deckCount;
            
            public DealShould()
            {
                deck = new CardCollectionBuilder().Build();
                deckCount = deck.Count();
                rules = new Rules();
                sut = new Dealer(rules);
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player2 = sampleData.Player2;
                player3 = sampleData.Player3;
            }

            [Fact]
            public void DealCardsToPlayer1()
            {
                var actual = sut.Deal(new[] {player1, player2, player3 }, deck);

                actual.Players[0].Hand.Cards.Should().Equal(
                    new Card(1, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(13, Suit.Clubs),
                    new Card(3, Suit.Diamonds),
                    new Card(6, Suit.Diamonds)
                );
            }

            [Fact]
            public void DealCardsToPlayer2()
            {
                var actual = sut.Deal(new[] {player1, player2, player3 }, deck);

                actual.Players[1].Hand.Cards.Should().Equal(
                    new Card(2, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(8, Suit.Clubs),
                    new Card(11, Suit.Clubs),
                    new Card(1, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(7, Suit.Diamonds)
                );
            }
            
            [Fact]
            public void DealCardsToPlayer3()
            {
                var actual = sut.Deal(new[] {player1, player2, player3}, deck);

                actual.Players[2].Hand.Cards.Should().Equal(
                    new Card(3, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(9, Suit.Clubs),
                    new Card(12, Suit.Clubs),
                    new Card(2, Suit.Diamonds),
                    new Card(5, Suit.Diamonds),
                    new Card(8, Suit.Diamonds)
                );
            }

            [Fact]
            public void MoveCardToDiscardPile()
            {
                var actual = sut.Deal(new[] {player1, player2}, deck);

                actual.StockPile.Cards.Count().Should().Be(deckCount - rules.GetHandSize() * 2 - 1);
                actual.DiscardPile.CardToMatch.Should().Be(new Card(2, Suit.Diamonds));
            }
        }
    }
}
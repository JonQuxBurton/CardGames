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
            private readonly int deckCount;
            
            public DealShould()
            {
                deck = new CardCollectionBuilder().Build();
                deckCount = deck.Count();
                rules = new Rules();
                sut = new Dealer(rules);
                player1 = new Player(1);
                player2 = new Player(2);
            }

            [Fact]
            public void DealCardsToPlayer1()
            {
                var actual = sut.Deal(new[] {player1, player2}, deck);

                actual.Player1.Hand.Cards.Should().Equal(
                    new Card(1, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(9, Suit.Clubs),
                    new Card(11, Suit.Clubs),
                    new Card(13, Suit.Clubs)
                );
            }

            [Fact]
            public void DealCardsToPlayer2()
            {
                var actual = sut.Deal(new[] {player1, player2}, deck);

                actual.Player2.Hand.Cards.Should().Equal(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(8, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(12, Suit.Clubs),
                    new Card(1, Suit.Diamonds)
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
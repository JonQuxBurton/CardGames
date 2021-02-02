using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace BoardTests
    {
        public class ConstructorShould
        {
            private readonly CardCollection expectedDiscardPile;
            private readonly Player expectedPlayer1;
            private readonly Player expectedPlayer2;
            private readonly StockPile expectedStockPile;
            private readonly Board sut;

            public ConstructorShould()
            {
                var expectedPlayer1Hand = new CardCollection(
                    new Card(1, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(9, Suit.Clubs),
                    new Card(13, Suit.Clubs),
                    new Card(13, Suit.Hearts)
                );

                var expectedPlayer2Hand = new CardCollection(
                new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(8, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(12, Suit.Clubs),
                    new Card(1, Suit.Diamonds)
                );

                expectedDiscardPile = new CardCollection(
                new Card(11, Suit.Diamonds),
                    new Card(13, Suit.Diamonds)
                );

                expectedStockPile = new StockPile(new CardCollection(new Card(1, Suit.Hearts), new Card(2, Suit.Hearts)));
                expectedPlayer1 = new Player(1) {Hand = expectedPlayer1Hand};
                expectedPlayer2 = new Player(2) {Hand = expectedPlayer2Hand};

                sut = new Board(expectedPlayer1, expectedPlayer2, expectedStockPile, new DiscardPile(expectedDiscardPile.Cards));
            }

            [Fact]
            public void SetPlayer1()
            {
                sut.Player1.Should().Be(expectedPlayer1);
            }

            [Fact]
            public void SetPlayer2()
            {
                sut.Player2.Should().Be(expectedPlayer2);
            }

            [Fact]
            public void SetStockPile()
            {
                sut.StockPile.Should().Be(expectedStockPile);
            }

            [Fact]
            public void SetDiscardPile()
            {
                sut.DiscardPile.RestOfCards.Cards.Should().Equal(expectedDiscardPile.Cards);
            }
        }

        public class TurnUpDiscardCardShould
        {
            private readonly Board sut;

            public TurnUpDiscardCardShould()
            {
                var originalStockPile = new StockPile(new CardCollection(
                    new Card(13, Suit.Diamonds),
                    new Card(1, Suit.Hearts)
                ));
                sut = new Board(new Player(1), new Player(2), originalStockPile, new DiscardPile());
                sut.TurnUpDiscardCard();
            }
            
            [Fact]
            public void RemoveCardFromStockPile()
            {
                sut.StockPile.Cards.Should().Equal(
                    new Card(1, Suit.Hearts)
                );
            }
            
            [Fact]
            public void TurnUpCard()
            {
                sut.DiscardPile.CardToMatch.Should().Be(
                    new Card(13, Suit.Diamonds)
                );
            }

            [Fact]
            public void AddToCardMoveEvent()
            {
                sut.CardMoves.Should().NotBeEmpty();
                var actual = sut.CardMoves.Last();
                actual.Card.Should().Be(new Card(13, Suit.Diamonds));
                actual.FromSource.Should().Be(CardMoveSources.StockPile);
                actual.ToSource.Should().Be(CardMoveSources.DiscardPile);
            }
        }
        
        public class MoveCardToDiscardPileShould
        {
            private readonly Board sut;
            private readonly Player player1;
            
            public MoveCardToDiscardPileShould()
            {
                player1 = new Player(1)
                {
                    Hand = new CardCollection(
                        new Card(1, Suit.Clubs), 
                        new Card(10, Suit.Hearts)
                    )
                };
                var originalDiscardPile = new CardCollection(new Card(5, Suit.Diamonds));
                sut = new Board(player1, new Player(2), new StockPile(new CardCollection()), new DiscardPile(originalDiscardPile.Cards));
            }
            
            [Fact]
            public void RemoveCardFromPlayersHand()
            {
                sut.MoveCardToDiscardPile(player1, new Card(10, Suit.Hearts));
                
                player1.Hand.Cards.Should().Equal(
                    new Card(1, Suit.Clubs)
                );
            }
            
            [Fact]
            public void AddCardToDiscardPile()
            {
                sut.MoveCardToDiscardPile(player1, new Card(10, Suit.Hearts));
                
                sut.DiscardPile.CardToMatch.Should().Be(new Card(10, Suit.Hearts));
                sut.DiscardPile.RestOfCards.Cards.Should().Equal(new Card(5, Suit.Diamonds));


            }

            [Theory]
            [InlineData(1, "Player1Hand")]
            [InlineData(2, "Player2Hand")]
            public void AddToCardMoveEvent(int playerNumber, string expectedSource)
            {
                var player = new Player(playerNumber)
                {
                    Hand = new CardCollection(
                        new Card(1, Suit.Clubs),
                        new Card(10, Suit.Hearts)
                    )
                };
                sut.MoveCardToDiscardPile(player, new Card(10, Suit.Hearts));

                sut.CardMoves.Should().NotBeEmpty();
                var actual = sut.CardMoves.Last();
                actual.Card.Should().Be(new Card(10, Suit.Hearts));
                actual.FromSource.Should().Be(expectedSource);
                actual.ToSource.Should().Be(CardMoveSources.DiscardPile);
            }
        }
        
        public class TakeCardFromStockPileShould
        {
            private readonly Board sut;
            private readonly Player player1;

            public TakeCardFromStockPileShould()
            {
                player1 = new Player(1)
                {
                    Hand = new CardCollection(new Card(1, Suit.Clubs))
                };
                var originalStockPile = new StockPile( new CardCollection(
                    new Card(5, Suit.Diamonds),
                    new Card(10, Suit.Hearts)
                ));
                sut = new Board(player1, new Player(2), originalStockPile, new DiscardPile());
            }
            
            [Fact]
            public void AddCardToPlayersHand()
            {
                sut.TakeCardFromStockPile(player1);
                
                sut.Player1.Hand.Cards.Should().Equal(
                    new Card(1, Suit.Clubs), 
                    new Card(5, Suit.Diamonds)
                );
            }
            
            [Fact]
            public void ReturnTakenCard()
            {
                var actual = sut.TakeCardFromStockPile(player1);
                
                actual.Should().Be(new Card(5, Suit.Diamonds));
            }
            
            [Fact]
            public void RemoveCardFromStockPile()
            {
                sut.TakeCardFromStockPile(player1);
                
                sut.StockPile.Cards.Should().Equal(
                    new Card(10, Suit.Hearts)
                );
            }
            
            [Theory]
            [InlineData(1, "Player1Hand")]
            [InlineData(2, "Player2Hand")]
            public void AddToCardMoveEvent(int playerNumber, string expectedSource)
            {
                var player = new Player(playerNumber)
                {
                    Hand = new CardCollection(
                        new Card(1, Suit.Clubs),
                        new Card(10, Suit.Hearts))
                };
                sut.TakeCardFromStockPile(player);

                sut.CardMoves.Should().NotBeEmpty();
                var actual = sut.CardMoves.Last();
                actual.Card.Should().Be(new Card(5, Suit.Diamonds));
                actual.FromSource.Should().Be(CardMoveSources.StockPile);
                actual.ToSource.Should().Be(expectedSource);
            }
        }

        public class MoveDiscardPileToStockPileShould
        {
            private readonly Board sut;

            public MoveDiscardPileToStockPileShould()
            {
                var player1 = new Player(1)
                {
                    Hand = new CardCollection(
                        new Card(1, Suit.Clubs),
                        new Card(10, Suit.Hearts)
                    )
                };
                var originalDiscardPile = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                );
                var discardPile = new DiscardPile(originalDiscardPile.Cards);
                discardPile.TurnUpTopCard();
                sut = new Board(player1, new Player(2), new StockPile(new CardCollection()), discardPile);
                sut.MoveDiscardPileToStockPile();
            }

            [Fact]
            public void RemoveCardsFromDiscardPile()
            {
                sut.DiscardPile.CardToMatch.Should().Be(new Card(1, Suit.Diamonds));
                sut.DiscardPile.RestOfCards.Cards.Should().BeEmpty();
            }
            
            [Fact]
            public void AddCardsToStockPile()
            {
                sut.StockPile.Cards.Should().Equal(
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                );
            }

            [Fact]
            public void AddToCardMoveEvents()
            {
                sut.CardMoves.Should().NotBeEmpty();
                sut.CardMoves.Should().Equal(
                    new CardMoveEvent(new Card(2, Suit.Diamonds), CardMoveSources.DiscardPile, CardMoveSources.StockPile),
                    new CardMoveEvent(new Card(3, Suit.Diamonds), CardMoveSources.DiscardPile, CardMoveSources.StockPile)
                );
            }
        }
    }
}
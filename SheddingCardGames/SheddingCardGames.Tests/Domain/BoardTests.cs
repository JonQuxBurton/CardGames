using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace BoardTests
    {
        public class BoardCreator
        {
            public static Board Create(StockPile stockPile, DiscardPile discardPile, params Player[] players)
            {
                return new Board(stockPile, discardPile, players);
            }
        }

        public class ConstructorShould
        {
            private readonly CardCollection expectedDiscardPile;
            private readonly Player expectedPlayer1;
            private readonly Player expectedPlayer2;
            private readonly Player expectedPlayer3;
            private readonly StockPile expectedStockPile;
            private readonly Board sut;

            public ConstructorShould()
            {
                var expectedPlayer1Hand = new CardCollection(
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs)
                );

                var expectedPlayer2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds)
                );
                
                var expectedPlayer3Hand = new CardCollection(
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Spades),
                    new Card(5, Suit.Spades)
                );

                expectedDiscardPile = new CardCollection(
                    new Card(11, Suit.Clubs),
                    new Card(13, Suit.Clubs)
                );

                var sampleData = new SampleData();
                expectedStockPile = new StockPile(new CardCollection(
                    new Card(1, Suit.Hearts), 
                    new Card(2, Suit.Hearts))
                );
                expectedPlayer1 = sampleData.Player1;
                expectedPlayer1.Hand = expectedPlayer1Hand;
                expectedPlayer2 = sampleData.Player2;
                expectedPlayer2.Hand = expectedPlayer2Hand;
                expectedPlayer3 = sampleData.Player3;
                expectedPlayer3.Hand = expectedPlayer3Hand;

                sut = BoardCreator.Create(expectedStockPile, new DiscardPile(expectedDiscardPile.Cards), expectedPlayer1, expectedPlayer2, expectedPlayer3);
            }

            [Fact]
            public void SetPlayer1()
            {
                sut.Players[0].Should().Be(expectedPlayer1);
            }

            [Fact]
            public void SetPlayer2()
            {
                sut.Players[1].Should().Be(expectedPlayer2);
            }            
            
            [Fact]
            public void SetPlayer3()
            {
                sut.Players[2].Should().Be(expectedPlayer3);
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

        public class AllCardsShould
        {
            private readonly Board sut;

            public AllCardsShould()
            {
                var player1Hand = new CardCollection(
                    new Card(1, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(5, Suit.Clubs)
                );

                var player2Hand = new CardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs)
                );
                
                var player3Hand = new CardCollection(
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades)
                );

                var discardPile = new CardCollection(
                    new Card(11, Suit.Diamonds),
                    new Card(13, Suit.Diamonds)
                );

                var sampleData = new SampleData();
                var stockPile = new StockPile(new CardCollection(
                    new Card(1, Suit.Hearts), 
                    new Card(2, Suit.Hearts)
                ));
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;

                sut = BoardCreator.Create(stockPile, new DiscardPile(discardPile.Cards), player1, player2, player3);
            }

            [Fact]
            public void ReturnAllCards()
    {
                var actual = sut.AllCards;

                actual.Cards.Should().Equal(
                    new Card(1, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(11, Suit.Diamonds),
                    new Card(13, Suit.Diamonds),
                    new Card(1, Suit.Hearts),
                    new Card(2, Suit.Hearts)
                    );
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
                var sampleData = new SampleData();
                sut = BoardCreator.Create(originalStockPile, new DiscardPile(), sampleData.Player1, sampleData.Player2);
            }
            
            [Fact]
            public void RemoveCardFromStockPile()
            {
                sut.TurnUpDiscardCard();

                sut.StockPile.Cards.Should().Equal(
                    new Card(1, Suit.Hearts)
                );
            }
            
            [Fact]
            public void TurnUpCard()
            {
                sut.TurnUpDiscardCard();

                sut.DiscardPile.CardToMatch.Should().Be(
                    new Card(13, Suit.Diamonds)
                );
            }

            [Fact]
            public void ReturnTurnedUpCard()
            {
                var actual= sut.TurnUpDiscardCard();

                actual.Should().Be(new Card(13, Suit.Diamonds));
            }
        }

        public class MoveCardToDiscardPileShould
        {
            private readonly Board sut;
            private readonly Player player1;
            
            public MoveCardToDiscardPileShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player1.Hand = new CardCollection(
                        new Card(1, Suit.Clubs), 
                        new Card(10, Suit.Hearts)
                );
                var originalDiscardPile = new CardCollection(new Card(5, Suit.Diamonds));
                sut = BoardCreator.Create(new StockPile(new CardCollection()), new DiscardPile(originalDiscardPile.Cards), player1, sampleData.Player2);
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

        }

        public class TakeCardFromStockPileShould
        {
            private readonly Board sut;
            private readonly Player player1;

            public TakeCardFromStockPileShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player1.Hand = new CardCollection(new Card(1, Suit.Clubs));
                var originalStockPile = new StockPile( new CardCollection(
                    new Card(5, Suit.Diamonds),
                    new Card(10, Suit.Hearts)
                ));
                sut = BoardCreator.Create(originalStockPile, new DiscardPile(), player1, sampleData.Player2);
            }
            
            [Fact]
            public void AddCardToPlayersHand()
            {
                sut.TakeCardFromStockPile(player1);
                
                sut.Players[0].Hand.Cards.Should().Equal(
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
        }

        public class MoveCardFromDiscardPileToStockPile
        {
            private readonly Board sut;

            public MoveCardFromDiscardPileToStockPile()
            {
                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = new CardCollection(
                        new Card(1, Suit.Clubs),
                        new Card(10, Suit.Hearts)
                    );
                var player2 = sampleData.Player2;
                var originalDiscardPile = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds)
                );
                var discardPile = new DiscardPile(originalDiscardPile.Cards);
                discardPile.TurnUpTopCard();
                sut = BoardCreator.Create(new StockPile(new CardCollection()), discardPile, player1, player2);
            }

            [Fact]
            public void RemoveCardFromDiscardPile()
            {
                sut.MoveCardFromDiscardPileToStockPile();

                sut.DiscardPile.CardToMatch.Should().Be(new Card(1, Suit.Diamonds));
                sut.DiscardPile.RestOfCards.Cards.Should().Equal(new Card(3, Suit.Diamonds));
            }

            [Fact]
            public void AddCardsToStockPile()
            {
                sut.MoveCardFromDiscardPileToStockPile();

                sut.StockPile.Cards.Should().Equal(
                    new Card(2, Suit.Diamonds)
                );
            }

            [Fact]
            public void ReturnMovedCard()
            {
                var actual = sut.MoveCardFromDiscardPileToStockPile();

                actual.Should().Be(new Card(2, Suit.Diamonds));
            }
        }

        //public class MoveDiscardPileToStockPileShould
        //{
        //    private readonly Board sut;

        //    public MoveDiscardPileToStockPileShould()
        //    {
        //        var sampleData = new SampleData();
        //        var player1 = sampleData.Player1;
        //        player1.Hand = new CardCollection(
        //                new Card(1, Suit.Clubs),
        //                new Card(10, Suit.Hearts)
        //            );
        //        var player2 = sampleData.Player2;
        //        var originalDiscardPile = new CardCollection(
        //            new Card(1, Suit.Diamonds),
        //            new Card(2, Suit.Diamonds),
        //            new Card(3, Suit.Diamonds)
        //        );
        //        var discardPile = new DiscardPile(originalDiscardPile.Cards);
        //        discardPile.TurnUpTopCard();
        //        sut = BoardCreator.Create(new StockPile(new CardCollection()), discardPile, player1, player2);
        //        sut.MoveDiscardPileToStockPile(new List<DomainEvent>());
        //    }

        //    [Fact]
        //    public void RemoveCardsFromDiscardPile()
        //    {
        //        sut.DiscardPile.CardToMatch.Should().Be(new Card(1, Suit.Diamonds));
        //        sut.DiscardPile.RestOfCards.Cards.Should().BeEmpty();
        //    }
            
        //    [Fact]
        //    public void AddCardsToStockPile()
        //    {
        //        sut.StockPile.Cards.Should().Equal(
        //            new Card(2, Suit.Diamonds),
        //            new Card(3, Suit.Diamonds)
        //        );
        //    }
        //}
    }
}
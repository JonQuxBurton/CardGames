using System.Collections.Immutable;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using Xunit;
using static SheddingCardGames.Domain.PlayersUtils;

namespace SheddingCardGames.Tests.Domain.CrazyEights
{
    namespace TableTests
    {
        public class ConstructorShould
        {
            private readonly CardCollection expectedDiscardPile;
            private readonly Player expectedPlayer1;
            private readonly Player expectedPlayer2;
            private readonly Player expectedPlayer3;
            private readonly StockPile expectedStockPile;
            private readonly Table sut;

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

                sut = TableCreator.Create(expectedStockPile, new DiscardPile(expectedDiscardPile.Cards), Players(expectedPlayer1, expectedPlayer2, expectedPlayer3));
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
            private readonly Table sut;

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

                sut = TableCreator.Create(stockPile, new DiscardPile(discardPile.Cards), Players(player1, player2, player3));
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

        public class MoveCardFromStockPileToDiscardPileShould
        {
            private readonly Table sut;

            public MoveCardFromStockPileToDiscardPileShould()
            {
                var originalStockPile = new StockPile(new CardCollection(
                    new Card(13, Suit.Diamonds),
                    new Card(1, Suit.Hearts)
                ));
                var sampleData = new SampleData();
                sut = TableCreator.Create(originalStockPile, new DiscardPile(), Players(sampleData.Player1, sampleData.Player2));
            }
            
            [Fact]
            public void RemoveCardFromStockPile()
            {
                sut.MoveCardFromStockPileToDiscardPile();

                sut.StockPile.Cards.Should().Equal(
                    new Card(1, Suit.Hearts)
                );
            }
            
            [Fact]
            public void TurnUpCard()
            {
                sut.MoveCardFromStockPileToDiscardPile();

                sut.DiscardPile.CardToMatch.Should().Be(
                    new Card(13, Suit.Diamonds)
                );
            }

            [Fact]
            public void ReturnTurnedUpCard()
            {
                var actual= sut.MoveCardFromStockPileToDiscardPile();

                actual.Should().Be(new Card(13, Suit.Diamonds));
            }
        }

        public class MoveCardFromPlayerToDiscardPileShould
        {
            private readonly Table sut;
            private readonly Player player1;
            
            public MoveCardFromPlayerToDiscardPileShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player1.Hand = new CardCollection(
                        new Card(1, Suit.Clubs), 
                        new Card(10, Suit.Hearts)
                );
                var originalDiscardPile = new CardCollection(new Card(5, Suit.Diamonds));
                sut = TableCreator.Create(new StockPile(new CardCollection()), new DiscardPile(originalDiscardPile.Cards), Players(player1, sampleData.Player2));
            }
            
            [Fact]
            public void RemoveCardFromPlayersHand()
            {
                sut.MoveCardFromPlayerToDiscardPile(player1, new Card(10, Suit.Hearts));
                
                player1.Hand.Cards.Should().Equal(
                    new Card(1, Suit.Clubs)
                );
            }
            
            [Fact]
            public void AddCardToDiscardPile()
            {
                sut.MoveCardFromPlayerToDiscardPile(player1, new Card(10, Suit.Hearts));
                
                sut.DiscardPile.CardToMatch.Should().Be(new Card(10, Suit.Hearts));
                sut.DiscardPile.RestOfCards.Cards.Should().Equal(new Card(5, Suit.Diamonds));
            }

        }

        public class MoveCardsFromPlayerToDiscardPileShould
        {
            private readonly Table sut;
            private readonly Player player1;
            
            public MoveCardsFromPlayerToDiscardPileShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player1.Hand = new CardCollection(
                        new Card(1, Suit.Clubs), 
                        new Card(1, Suit.Spades),
                        new Card(1, Suit.Hearts),
                        new Card(9, Suit.Diamonds)
                );
                var originalDiscardPile = new CardCollection(
                    new Card(1, Suit.Diamonds)
                    );
                sut = TableCreator.Create(new StockPile(new CardCollection()), new DiscardPile(originalDiscardPile.Cards), Players(player1, sampleData.Player2));
            }
            
            [Fact]
            public void RemoveCardsFromPlayersHand()
            {
                sut.MoveCardsFromPlayerToDiscardPile(player1, ImmutableList.Create(
                    new Card(1, Suit.Clubs),
                    new Card(1, Suit.Spades),
                    new Card(1, Suit.Hearts)
                ));
                player1.Hand.Cards.Should().Equal(
                    new Card(9, Suit.Diamonds)
                );
            }
            
            [Fact]
            public void AddCardsToDiscardPile()
            {
                sut.MoveCardsFromPlayerToDiscardPile(player1, ImmutableList.Create(
                    new Card(1, Suit.Clubs),
                    new Card(1, Suit.Spades),
                    new Card(1, Suit.Hearts)
                ));

                sut.DiscardPile.CardToMatch.Should().Be(new Card(1, Suit.Hearts));
                sut.DiscardPile.RestOfCards.Cards.Should().Equal(
                    new Card(1, Suit.Spades),
                    new Card(1, Suit.Clubs),
                    new Card(1, Suit.Diamonds)
                    );
            }
        }

        public class MoveCardFromStockPileToPlayerShould
        {
            private readonly Table sut;
            private readonly Player player1;

            public MoveCardFromStockPileToPlayerShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player1.Hand = new CardCollection(new Card(1, Suit.Clubs));
                var originalStockPile = new StockPile( new CardCollection(
                    new Card(5, Suit.Diamonds),
                    new Card(10, Suit.Hearts)
                ));
                sut = TableCreator.Create(originalStockPile, new DiscardPile(), Players(player1, sampleData.Player2));
            }
            
            [Fact]
            public void AddCardToPlayersHand()
            {
                sut.MoveCardFromStockPileToPlayer(player1);
                
                sut.Players[0].Hand.Cards.Should().Equal(
                    new Card(1, Suit.Clubs), 
                    new Card(5, Suit.Diamonds)
                );
            }
            
            [Fact]
            public void ReturnTakenCard()
            {
                var actual = sut.MoveCardFromStockPileToPlayer(player1);
                
                actual.Should().Be(new Card(5, Suit.Diamonds));
            }
            
            [Fact]
            public void RemoveCardFromStockPile()
            {
                sut.MoveCardFromStockPileToPlayer(player1);
                
                sut.StockPile.Cards.Should().Equal(
                    new Card(10, Suit.Hearts)
                );
            }
        }

        public class MoveCardFromDiscardPileToStockPile
        {
            private readonly Table sut;

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
                sut = TableCreator.Create(new StockPile(new CardCollection()), discardPile, Players(player1, player2));
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
    }
}
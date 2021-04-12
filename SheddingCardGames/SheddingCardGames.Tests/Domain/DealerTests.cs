using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
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
                var actual = sut.Deal(new[] {player1, player2, player3 }, deck, new List<DomainEvent>());

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
                var actual = sut.Deal(new[] {player1, player2, player3 }, deck, new List<DomainEvent>());

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
                var actual = sut.Deal(new[] {player1, player2, player3}, deck, new List<DomainEvent>());

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
                var actual = sut.Deal(new[] {player1, player2}, deck, new List<DomainEvent>());

                actual.StockPile.Cards.Count().Should().Be(deckCount - rules.GetHandSize() * 2 - 1);
                actual.DiscardPile.CardToMatch.Should().Be(new Card(2, Suit.Diamonds));
            }
            
            [Fact]
            public void AddEventForTurnUpDiscardCardEvent()
            {
                var actualEvents = new List<DomainEvent>();
                sut.Deal(new[] {player1, player2}, deck, actualEvents);

                actualEvents.Last().Should().BeOfType<CardMoved>();
                var domainEvent = actualEvents.Last() as CardMoved;
                domainEvent.Card.Should().Be(new Card(2, Suit.Diamonds));
                domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.DiscardPile);
            }
            
            [Fact]
            public void AddEventsForDeal()
            {
                var actualEvents = new List<DomainEvent>();
                sut.Deal(new[] {player1, player2}, deck, actualEvents);

                for (int i = 0; i < 7; i++)
                {
                    var counter1 = i * 2;
                    actualEvents.ElementAt(counter1).Should().BeOfType<CardMoved>();
                    var domainEvent = actualEvents.ElementAt(counter1) as CardMoved;
                    domainEvent.Card.Should().Be(player1.Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(1));

                    var counter2 = (i * 2) + 1;
                    actualEvents.ElementAt(counter2).Should().BeOfType<CardMoved>();
                    domainEvent = actualEvents.ElementAt(counter2) as CardMoved;
                    domainEvent.Card.Should().Be(player2.Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(2));
                }
            }
        }
    }
}
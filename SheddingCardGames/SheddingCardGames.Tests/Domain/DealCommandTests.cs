using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CrazyEightsRules;
using static SheddingCardGames.Domain.PlayersUtils;

namespace SheddingCardGames.Tests.Domain
{
    namespace DealCommandTests
    {
        public class IsValidShould
        {
            private DealCommand CreateSut(DiscardPile discardPile, CardCollection player1Hand)
            {
                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;

                discardPile.TurnUpTopCard();
                var players = Players(player1, sampleData.Player2);
                var table = TableCreator.Create(new StockPile(new CardCollection()), discardPile, players);
                var gameState = new GameState(players)
                {
                    CurrentTable = table,
                    PlayerToStart = player1,
                };

                var deck = new CardCollectionBuilder().Build();
                var rules = new CrazyEightsRules(NumberOfPlayers.Two);

                return new DealCommand(rules, 
                    new DummyShuffler(), 
                    gameState, new DealContext(deck));
            }

            [Fact]
            public void ReturnIsSuccessTrue()
            {
                var playedCard = new Card(1, Suit.Clubs);
                var player1Hand = new CardCollection(playedCard);
                var discardPile = new DiscardPile(new CardCollection(
                    new Card(2, Suit.Clubs)
                ));

                var sut = CreateSut(discardPile, player1Hand);

                var actual = sut.IsValid();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }
        }

        public class ExecuteShould
        {
            private int deckCount;
            private CardCollection player1Hand;
            private CrazyEightsRules rules;
            private readonly ImmutableList<Player> players;

            public ExecuteShould()
            {
                var sampleData = new SampleData();
                players = Players(sampleData.Player1, sampleData.Player2, sampleData.Player3);

                player1Hand = new CardCollection(
                    new Card(1, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(9, Suit.Clubs)
                );
            }

            private DealCommand CreateSut(Player currentPlayer)
            {
                var table = TableCreator.Create(new StockPile(new CardCollection()), new DiscardPile(), players);
                var gameState = new GameState(players)
                {
                    CurrentTable = table,
                    PlayerToStart = currentPlayer,
                    CurrentTurn = new CurrentTurn(1, currentPlayer, Action.Play)
                };

                var player2Hand = new CardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(9, Suit.Hearts),
                    new Card(10, Suit.Clubs)
                );
                var player3Hand = new CardCollection(
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Spades),
                    new Card(9, Suit.Spades)
                );
                var discardCard = new Card(9, Suit.Diamonds);
                var stockPile = new CardCollection(
                    new Card(1, Suit.Hearts),
                    new Card(2, Suit.Hearts),
                    new Card(3, Suit.Hearts)
                );

                var deck =
                    new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand).Build();
                deckCount = deck.Count();

                rules = new CrazyEightsRules(NumberOfPlayers.Three);

                return new DealCommand(rules, new DummyShuffler(), gameState, new DealContext(deck));
            }

            [Fact]
            public void DealCardsToPlayer1()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                actual.CurrentTable.Players[0].Hand.Cards.Should().Equal(
                    new Card(1, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(9, Suit.Clubs)
                );
            }

            [Fact]
            public void DealCardsToPlayer2()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                actual.CurrentTable.Players[1].Hand.Cards.Should().Equal(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(9, Suit.Hearts),
                    new Card(10, Suit.Clubs)
                );
            }

            [Fact]
            public void DealCardsToPlayer3()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                actual.CurrentTable.Players[2].Hand.Cards.Should().Equal(
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Spades),
                    new Card(9, Suit.Spades)
                );
            }

            [Fact]
            public void MoveCardToDiscardPile()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                actual.CurrentTable.StockPile.Cards.Count().Should()
                    .Be(deckCount - rules.GetHandSize() * players.Count - 1);
                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(new Card(9, Suit.Diamonds));
            }

            [Fact]
            public void AddEventForTurnUpDiscardCardEvent()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                var actualEvent = actual.Events.LastSkip(1);
                actualEvent.Should().BeOfType<CardMoved>();
                var domainEvent = actualEvent as CardMoved;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Card.Should().Be(new Card(9, Suit.Diamonds));
                domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.DiscardPile);
            }

            [Fact]
            public void AddEventsForDeal()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                var actualEvents = actual.Events.ToArray();
                for (var i = 0; i < rules.GetHandSize(); i++)
                {
                    var counter1 = i * players.Count;
                    actualEvents.ElementAt(counter1).Should().BeOfType<CardMoved>();
                    var domainEvent = actualEvents.ElementAt(counter1) as CardMoved;
                    if (domainEvent is null) Assert.NotNull(domainEvent);
                    domainEvent.Card.Should().Be(players[0].Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(1));

                    var counter2 = i * players.Count + 1;
                    actualEvents.ElementAt(counter2).Should().BeOfType<CardMoved>();
                    domainEvent = actualEvents.ElementAt(counter2) as CardMoved;
                    if (domainEvent is null) Assert.NotNull(domainEvent);
                    domainEvent.Card.Should().Be(players[1].Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(2));

                    var counter3 = i * players.Count + 2;
                    actualEvents.ElementAt(counter3).Should().BeOfType<CardMoved>();
                    domainEvent = actualEvents.ElementAt(counter3) as CardMoved;
                    if (domainEvent is null) Assert.NotNull(domainEvent);
                    domainEvent.Card.Should().Be(players[2].Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(3));
                }
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer1()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.NextAction.Should().Be(Action.Play);

                actual.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer2()
            {
                var sut = CreateSut(players[1]);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.NextAction.Should().Be(Action.Play);

                actual.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer3()
            {
                var sut = CreateSut(players[2]);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(3);
                actualTurn.NextAction.Should().Be(Action.Play);

                actual.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWithNextActionTake()
            {
                player1Hand = new CardCollection(
                    new Card(1, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(7, Suit.Clubs),
                    new Card(11, Suit.Clubs)
                );
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.NextAction.Should().Be(Action.Take);

                actual.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void ReturnGameStatePreviousTurnResultNull()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                actual.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void ReturnGameStateWithCurrentGamePhaseReadyToInGame()
            {
                var sut = CreateSut(players[0]);

                var actual = sut.Execute();

                actual.CurrentGamePhase.Should().Be(GamePhase.InGame);
            }

        }
    }
}
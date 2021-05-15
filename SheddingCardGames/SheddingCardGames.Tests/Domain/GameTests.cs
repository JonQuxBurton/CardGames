using System.Linq;
using FluentAssertions;
using Moq;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain
{
    namespace GameTests
    {
        public class ConstructorShould
        {
            [Fact]
            public void CreateGameAtNewState()
            {
                var sampleData = new SampleData();
                var rules = new CrazyEightsRules(NumberOfPlayers.Two);
                
                var sut = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, new DummyShuffler())), new CardCollectionBuilder().Build(), new [] { sampleData.Player1, sampleData.Player2, sampleData.Player3 });

                sut.GameState.CurrentTurn.Should().BeNull();
                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.New);
                sut.GameState.Events.Should().BeEmpty();
            }
        }

        public class InitialiseShould
        {
            private readonly Game sut;

            public InitialiseShould()
            {
                var sampleData = new SampleData();
                var rules = new CrazyEightsRules(NumberOfPlayers.Two);
                sut = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, new DummyShuffler())), new CardCollectionBuilder().Build(),
                    new[] { sampleData.Player1, sampleData.Player2 });
            }

            [Fact]
            public void SetGameState()
            {
                var expectedGameState = new GameState();

                sut.Initialise(expectedGameState);

                sut.GameState.Should().Be(expectedGameState);
            }
        }
        
        public class ChooseStartingPlayerShould
        {
            private readonly Game sut;
            private readonly Player player1;
            private readonly Player player2;
            private readonly Player player3;

            public ChooseStartingPlayerShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player2 = sampleData.Player2;
                player3 = sampleData.Player3;

                var rules = new CrazyEightsRules(NumberOfPlayers.Two);
                var deck = new DeckBuilder().Build();
                sut = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, new DummyShuffler())), deck, new[] { player1, player2, player3 });
            }

            [Fact]
            public void ReturnTrueWhenMatchingSuit()
            {
                var actual = sut.ChooseStartingPlayer(new ChooseStartingPlayerContext(player1));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void CreateStartingPlayerChosenEvent()
            {
                sut.ChooseStartingPlayer(new ChooseStartingPlayerContext(player2));

                sut.GameState.Events.Last().Number.Should().Be(1);
                sut.GameState.Events.Last().Should().BeOfType(typeof(StartingPlayerChosen));
                var startingPlayerChosenEvent = sut.GameState.Events.Last() as StartingPlayerChosen;
                startingPlayerChosenEvent.Player.Number.Should().Be(2);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public void SetPlayerToStart(int startingPlayer)
            {
                var sampleData = new SampleData();
                sut.ChooseStartingPlayer(new ChooseStartingPlayerContext(sampleData.GetPlayer(startingPlayer)));
                
                sut.GameState.PlayerToStart.Number.Should().Be(startingPlayer);
            }

            [Fact]
            public void SetGamePhaseToReadyToDeal1()
            {
                sut.ChooseStartingPlayer(new ChooseStartingPlayerContext(player1));
                
                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.ReadyToDeal);
            }
        }

        public class GetCardShould
        {
            private readonly Game sut;

            public GetCardShould()
            {
                var player1Hand = new CardCollection(
                    new Card(1, Clubs),
                    new Card(2, Clubs),
                    new Card(3, Clubs),
                    new Card(4, Clubs),
                    new Card(5, Clubs),
                    new Card(6, Clubs),
                    new Card(7, Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Diamonds),
                    new Card(2, Diamonds),
                    new Card(3, Diamonds),
                    new Card(4, Diamonds),
                    new Card(5, Diamonds),
                    new Card(6, Diamonds),
                    new Card(7, Diamonds)
                );
                var player3Hand = new CardCollection(
                    new Card(1, Spades),
                    new Card(2, Spades),
                    new Card(3, Spades),
                    new Card(4, Spades),
                    new Card(5, Spades),
                    new Card(6, Spades),
                    new Card(7, Spades)
                );
                var stockPile = new CardCollection(new Card(1, Hearts));
                var discardCard = new Card(2, Hearts);

                sut = new ReadyToDealGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .Build();
            }

            [Fact]
            public void ReturnCardFromStockPile()
            {
                var actual = sut.GetCard(1, Spades);

                actual.Should().Be(new Card(1, Spades));
            }
            
            [Fact]
            public void ReturnCardFromDiscardPile()
            {
                var actual = sut.GetCard(2, Hearts);

                actual.Should().Be(new Card(2, Hearts));
            }
            
            [Fact]
            public void ReturnCardFromPlayer1Hand()
            {
                var actual = sut.GetCard(7, Clubs);

                actual.Should().Be(new Card(7, Clubs));
            }
            
            [Fact]
            public void ReturnCardFromPlayer2Hand()
            {
                var actual = sut.GetCard(4, Diamonds);

                actual.Should().Be(new Card(4, Diamonds));
            }
            
            [Fact]
            public void ReturnCardFromPlayer3Hand()
            {
                var actual = sut.GetCard(2, Spades);

                actual.Should().Be(new Card(2, Spades));
            }
            
            [Fact]
            public void ReturnNullWhenCardNotFound()
            {
                var actual = sut.GetCard(13, Spades);

                actual.Should().BeNull();
            }
        }

        public class DealShould
        {
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private readonly CardCollection player3Hand;
            private readonly CardCollection stockPile;

            public DealShould()
            {
                player1Hand = new CardCollection(
                    new Card(1, Clubs),
                    new Card(2, Clubs),
                    new Card(3, Clubs),
                    new Card(4, Clubs),
                    new Card(5, Clubs)
                );
                player2Hand = new CardCollection(
                    new Card(1, Diamonds),
                    new Card(2, Diamonds),
                    new Card(3, Diamonds),
                    new Card(4, Diamonds),
                    new Card(5, Diamonds)
                );
                player3Hand = new CardCollection(
                    new Card(1, Spades),
                    new Card(2, Spades),
                    new Card(3, Spades),
                    new Card(4, Spades),
                    new Card(5, Spades)
                );
                stockPile = new CardCollection(new Card(1, Hearts));
            }

            private Game CreateSut(int withStartingPlayer, Card withDiscardCard, IShuffler withShuffler = null)
            {
                if (withShuffler == null)
                    withShuffler = new DummyShuffler();
                
                return new ReadyToDealGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(withDiscardCard)
                    .WithStartingPlayer(withStartingPlayer)
                    .WithShuffler(withShuffler)
                    .Build();
            }

            [Fact]
            public void ReturnTrue()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                var actual = sut.Deal();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void CreateCardMovedEvents()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                CardMoved domainEvent;

                for (int i = 0; i < 5; i++)
                {
                    var eventIndex = i * 3;

                    sut.GameState.Events.ElementAt(eventIndex + 1).Should().BeOfType(typeof(CardMoved));
                    domainEvent = sut.GameState.Events.ElementAt(eventIndex + 1) as CardMoved;
                    if (domainEvent == null) Assert.NotNull(domainEvent);
                    domainEvent.Card.Should().Be(player1Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(1));
                    
                    sut.GameState.Events.ElementAt(eventIndex + 2).Should().BeOfType(typeof(CardMoved));
                    domainEvent = sut.GameState.Events.ElementAt(eventIndex + 2) as CardMoved;
                    if (domainEvent == null) Assert.NotNull(domainEvent);
                    domainEvent.Card.Should().Be(player2Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(2));

                    sut.GameState.Events.ElementAt(eventIndex + 3).Should().BeOfType(typeof(CardMoved));
                    domainEvent = sut.GameState.Events.ElementAt(eventIndex + 3) as CardMoved;
                    if (domainEvent == null) Assert.NotNull(domainEvent);
                    domainEvent.Card.Should().Be(player3Hand.Cards.ElementAt(i));
                    domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                    domainEvent.ToSource.Should().Be(CardMoveSources.PlayerHand(3));
                }

                sut.GameState.Events.LastSkip(1).Should().BeOfType(typeof(CardMoved));
                domainEvent = sut.GameState.Events.LastSkip(1) as CardMoved;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Card.Should().Be(discardCard);
                domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.DiscardPile);
            }
            
            [Fact]
            public void CreateDealCompletedEvent()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.Events.Last().Should().BeOfType(typeof(DealCompleted));
            }

            [Fact]
            public void ShuffleCards()
            {
                var discardCard = new Card(2, Hearts);
                var expectedDeck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand).Build();
                var shufflerMock = new Mock<IShuffler>();
                shufflerMock.Setup(x => x.Shuffle(It.Is<CardCollection>(y => y.Cards.SequenceEqual(expectedDeck.Cards) )))
                    .Returns(expectedDeck);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard, shufflerMock.Object);

                sut.Deal();
                
                shufflerMock.VerifyAll();
            }
            
            [Fact]
            public void SetGameStateToInGame()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);
                
                sut.Deal();

                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.InGame);
            }
            
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public void PreserveStartingPlayer(int expectedStartingPlayer)
            {
                var discardCard = new Card(2, Hearts);
                var sut = CreateSut(expectedStartingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentTurn.PlayerToPlay.Number.Should().Be(expectedStartingPlayer);
            }
            
            [Fact]
            public void AddHandToPlayer1()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            }

            [Fact]
            public void AddHandToPlayer2()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                var actual = sut.GameState.CurrentTable;
                actual.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            }
            
            [Fact]
            public void AddHandToPlayer3()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                var actual = sut.GameState.CurrentTable;
                actual.Players[2].Hand.Cards.Should().Equal(player3Hand.Cards);
            }

            [Fact]
            public void SetupDiscardPile()
            {
                var expectedDiscardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, expectedDiscardCard);

                sut.Deal();

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(expectedDiscardCard);
            }

            [Fact]
            public void SetupStockPile()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(stockPile.Cards);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer1()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer2()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 2;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();
                
                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer3()
            {
                var discardCard = new Card(2, Hearts);
                var startingPlayer = 3;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWithNextActionTake()
            {
                var discardCard = new Card(10, Spades);
                var startingPlayer = 1;
                var sut = new ReadyToDealGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(startingPlayer)
                    .Build();

                sut.Deal();

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Take);
            }
        }

        public class PlayWhenValidShould
        {
            private readonly CardCollection deck;
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private readonly CardCollection player3Hand;
            private Game sut;

            public PlayWhenValidShould()
            {
                deck = new CardCollectionBuilder().Build();

                player1Hand = new CardCollection(
                    deck.Get(2, Clubs),
                    deck.Get(3, Clubs),
                    deck.Get(4, Clubs),
                    deck.Get(10, Spades)
                );
                player2Hand = new CardCollection(
                    deck.Get(1, Diamonds),
                    deck.Get(2, Diamonds),
                    deck.Get(3, Diamonds),
                    deck.Get(4, Diamonds),
                    deck.Get(5, Diamonds)
                );
                player3Hand = new CardCollection(
                    deck.Get(1, Spades),
                    deck.Get(2, Spades),
                    deck.Get(3, Spades),
                    deck.Get(4, Spades),
                    deck.Get(5, Spades)
                );
            }

            [Fact]
            public void ReturnTrueWhenMatchingSuit()
            {
                var discardCard = deck.Get(10, Clubs);
                var playedCard = deck.Get(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();
                
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnTrueWhenMatchingRank()
            {
                var discardCard = deck.Get(10, Clubs);
                var playedCard = deck.Get(10, Hearts);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnTrueWhenRank8()
            {
                var discardCard = deck.Get(10, Hearts);
                var playedCard = deck.Get(8, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void AddCardToDiscardPile()
            {
                var discardCard = deck.Get(1, Hearts);
                var playedCard = deck.Get(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard);
            }

            [Fact]
            public void RemoveCardFromPlayersHand()
            {
                var discardCard = Card(1, Hearts);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().NotContain(playedCard);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var discardCard = deck.Get(1, Hearts);
                var playedCard = deck.Get(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().NotContain(playedCard);
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard);
            }

            [Fact]
            public void CreateNewTurnWithNextActionTake()
            {
                var discardCard = deck.Get(1, Hearts);
                var playedCard = deck.Get(1, Clubs);
                var player2HandWithNoPlays = new CardCollection(
                    deck.Get(2, Diamonds),
                    deck.Get(3, Diamonds),
                    deck.Get(4, Diamonds),
                    deck.Get(5, Diamonds),
                    deck.Get(6, Diamonds)
                );
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2HandWithNoPlays)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.NextAction.Should().Be(Action.Take);
            }

            [Fact]
            public void CreateNewTurnAfterPlayer2TurnWithValidPlay()
            {
                var discardCard = deck.Get(1, Hearts);
                var playedCard = deck.Get(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                var playedCard2 = deck.Get(1, Diamonds);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));
                sut.Play(new PlayContext(sut.GetPlayer(2), playedCard2));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(3);
                actual.PlayerToPlay.Number.Should().Be(3);
                actual.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();

                sut.GameState.CurrentTable.Players[1].Hand.Cards.Should().NotContain(playedCard2);
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard2);
            }
            
            [Fact]
            public void CreateNewTurnAfterPlayer3TurnWithValidPlay()
            {
                var discardCard = deck.Get(10, Clubs);
                var playedCard = deck.Get(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                var playedCard2 = deck.Get(1, Diamonds);
                var playedCard3 = deck.Get(1, Spades);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));
                sut.Play(new PlayContext(sut.GetPlayer(2), playedCard2));
                sut.Play(new PlayContext(sut.GetPlayer(3), playedCard3));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(4);
                actual.PlayerToPlay.Number.Should().Be(1);
                actual.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeFalse();
                actualPreviousTurnResult.Winner.Should().BeNull();

                sut.GameState.CurrentTable.Players[2].Hand.Cards.Should().NotContain(playedCard3);
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard3);
            }

            [Fact]
            public void CreatePlayedEvent()
            {
                var discardCard = deck.Get(10, Clubs);
                var playedCard = deck.Get(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                sut.GameState.Events.Last().Should().BeOfType(typeof(Played));
                var played = sut.GameState.Events.Last() as Played;
                if (played == null) Assert.NotNull(played);
                played.PlayerNumber.Should().Be(1);
                played.Cards.Should().Equal(playedCard);
            }
        }

        public class PlayWhenPlayingEightShould
        {
            private Game sut;

            public PlayWhenPlayingEightShould()
            {
                new CardCollectionBuilder().Build();

                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(8, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds)
                );
                var player3Hand = new CardCollection(
                    Card(1, Spades),
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .Build();
            }

            [Fact]
            public void ReturnTrue()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), Card(8, Clubs)));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void CreateCrazyEightTurn()
            {
                var cardToPlay = Card(8, Clubs);

                sut.Play(new PlayContext(sut.GetPlayer(1), cardToPlay));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.PlayerToPlay.Number.Should().Be(1);
                actual.NextAction.Should().Be(Action.SelectSuit);
                
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardToPlay);
            }
            
            [Fact]
            public void CreateCrazyEightTurnWhenTurn2()
            {
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(8, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(8, Diamonds),
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds)
                );
                var player3Hand = new CardCollection(
                    Card(8, Spades),
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .Build();
                sut.Play(new PlayContext(sut.GetPlayer(1), Card(1, Clubs)));

                sut.Play(new PlayContext(sut.GetPlayer(2), Card(8, Diamonds)));
                
                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.NextAction.Should().Be(Action.SelectSuit);
                
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(Card(8, Diamonds));
            }
            
            [Fact]
            public void CreateCrazyEightTurnWhenTurn3()
            {
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(8, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds)
                );
                var player3Hand = new CardCollection(
                    Card(8, Spades),
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), Card(1, Clubs)));
                sut.Play(new PlayContext(sut.GetPlayer(2), Card(1, Diamonds)));

                sut.Play(new PlayContext(sut.GetPlayer(3), Card(8, Spades)));
                
                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(3);
                actual.PlayerToPlay.Number.Should().Be(3);
                actual.NextAction.Should().Be(Action.SelectSuit);
                
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(Card(8, Spades));
            }
        }

        public class PlayWhenInvalidShould
        {
            private readonly CardCollection deck;
            private readonly Card originalDiscardCard;
            private readonly Card playedCard;
            private readonly Game sut;

            public PlayWhenInvalidShould()
            {
                deck = new CardCollectionBuilder().Build();

                originalDiscardCard = deck.Get(10, Hearts);
                var player1Hand = new CardCollection(
                    deck.Get(1, Clubs),
                    deck.Get(2, Clubs),
                    deck.Get(3, Clubs),
                    deck.Get(4, Clubs),
                    deck.Get(7, Hearts)
                );
                var player2Hand = new CardCollection(
                    deck.Get(1, Diamonds),
                    deck.Get(2, Diamonds),
                    deck.Get(3, Diamonds),
                    deck.Get(7, Diamonds),
                    deck.Get(10, Diamonds)
                );
                var player3Hand = new CardCollection(
                    deck.Get(1, Spades),
                    deck.Get(2, Spades),
                    deck.Get(3, Spades),
                    deck.Get(7, Spades),
                    deck.Get(10, Spades)
                );
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                playedCard = deck.Get(1, Clubs);

                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(originalDiscardCard)
                    .WithStockPile(stockPile)
                    .Build();
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer1Hand()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), deck.Get(1, Hearts)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer2Hand()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), deck.Get(7, Hearts)));
                var actual = sut.Play(new PlayContext(sut.GetPlayer(2), deck.Get(1, Hearts)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer3Hand()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), deck.Get(7, Hearts)));
                sut.Play(new PlayContext(sut.GetPlayer(2), deck.Get(7, Diamonds)));
                var actual = sut.Play(new PlayContext(sut.GetPlayer(3), deck.Get(1, Hearts)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs1AndIsNotCurrentPlayer()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), deck.Get(7, Hearts)));
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), deck.Get(7, Diamonds)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs2AndIsNotCurrentPlayer()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(2), deck.Get(10, Diamonds)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs3AndIsNotCurrentPlayer()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(3), deck.Get(10, Spades)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayIsInvalid()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void NotAddCardToDiscardPile()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(originalDiscardCard);
            }

            [Fact]
            public void NotRemoveCardFromPlayersHand()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Contain(playedCard);
            }

            [Fact]
            public void NotCreateNewTurn()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(1);
            }
        }

        public class SelectSuitShould
        {
            private readonly Game sut;

            public SelectSuitShould()
            {
                var player1Hand = new CardCollection(
                    Card(8, Clubs),
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(5, Clubs),
                    Card(6, Clubs)
                );
                var player2Hand = new CardCollection(
                    Card(5, Diamonds),
                    Card(6, Diamonds),
                    Card(7, Diamonds),
                    Card(8, Diamonds),
                    Card(9, Diamonds),
                    Card(10, Diamonds),
                    Card(11, Diamonds)
                );

                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .Build();
                sut.Play(new PlayContext(sut.GetPlayer(1), Card(8, Clubs)));
            }

            [Fact]
            public void AddSelectedSuitToTurn()
            {
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(1), Diamonds));

                var actualCurrentTurn = sut.GameState.CurrentTurn;
                actualCurrentTurn.TurnNumber.Should().Be(2);
                actualCurrentTurn.PlayerToPlay.Number.Should().Be(2);
                actualCurrentTurn.NextAction.Should().Be(Action.Play);

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.SelectedSuit.Should().Be(Diamonds);
            }

            [Fact]
            public void KeepSelectedSuitOnTurnAfterTake()
            {
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(1), Diamonds));

                sut.Take(new TakeContext(sut.GetPlayer(2)));

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.SelectedSuit.Should().Be(Diamonds);
            }

            [Fact]
            public void ReturnTrueWhenTakeIsValid()
            {
                var actual = sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(1), Diamonds));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnFalseWhenNotPlayersTurn()
            {
                var actual = sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(2), Diamonds));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void CreateSuitSelectedEvent()
            {
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(1), Diamonds));

                sut.GameState.Events.Last().Should().BeOfType(typeof(SuitSelected));
                var domainEvent = sut.GameState.Events.Last() as SuitSelected;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
                domainEvent.Suit.Should().Be(Diamonds);
            }

        }

        public class PlayWhenSelectedSuitShould
        {
            private readonly Card discardCard;
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private readonly CardCollection player3Hand;
            private Game sut;

            public PlayWhenSelectedSuitShould()
            {
                player1Hand = new CardCollection
                (
                    new Card(1, Clubs),
                    new Card(2, Clubs),
                    new Card(3, Clubs),
                    new Card(4, Clubs),
                    new Card(8, Clubs)
                );
                player2Hand = new CardCollection
                (
                    new Card(1, Diamonds),
                    new Card(2, Diamonds),
                    new Card(3, Diamonds),
                    new Card(4, Diamonds),
                    new Card(8, Diamonds)
                );
                player3Hand = new CardCollection
                (
                    new Card(1, Spades),
                    new Card(2, Spades),
                    new Card(3, Spades),
                    new Card(4, Spades),
                    new Card(8, Spades)
                );

                discardCard = new Card(1, Hearts);
            }

            [Fact]
            public void ReturnTrue_ForPlayMatchingSelectedSuit()
            {
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), new Card(8, Clubs)));
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(1), Diamonds));

                var actual = sut.Play(new PlayContext(sut.GetPlayer(2), new Card(1, Diamonds)));
                actual.IsSuccess.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnTrue_ForPlayMatchingSelectedSuitAfterPlayer2()
            {
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(2)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(2), new Card(8, Diamonds)));
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(2), Spades));

                var actual = sut.Play(new PlayContext(sut.GetPlayer(3), new Card(1, Spades)));
                actual.IsSuccess.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnTrue_ForPlayMatchingSelectedSuitAfterPlayer3()
            {
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(3)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(3), new Card(8, Spades)));
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(3), Clubs));

                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), new Card(1, Clubs)));
                actual.IsSuccess.Should().BeTrue();
            }
        }

        public class TakeShould
        {
            private readonly CardCollection deck;
            private CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private CardCollection player3Hand;

            public TakeShould()
            {
                deck = new CardCollectionBuilder().Build();
                player1Hand = new CardCollection(
                    deck.Get(1, Clubs),
                    deck.Get(2, Clubs),
                    deck.Get(3, Clubs),
                    deck.Get(4, Clubs),
                    deck.Get(5, Clubs)
                );
                player2Hand = new CardCollection(
                    deck.Get(2, Diamonds),
                    deck.Get(3, Diamonds),
                    deck.Get(4, Diamonds),
                    deck.Get(5, Diamonds),
                    deck.Get(10, Diamonds)
                );
                player3Hand = new CardCollection(
                    deck.Get(2, Spades),
                    deck.Get(3, Spades),
                    deck.Get(4, Spades),
                    deck.Get(5, Spades),
                    deck.Get(6, Spades)
                );
            }

            [Fact]
            public void CreatesNewTurn()
            {
                var discardCard = deck.Get(10, Hearts);
                var stockPile = new CardCollection(
                    deck.Get(9, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(new TakeContext(sut.GetPlayer(1)));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.NextAction.Should().Be(Action.Play);
                
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            }

            [Fact]
            public void ReturnCardTaken()
            {
                var discardCard = deck.Get(10, Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(1)));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
                sut.GameState.PreviousTurnResult.TakenCard.Should().Be(new Card(1, Hearts));
            }

            [Fact]
            public void ReturnCardTakenWhenCardToMatchIsEightAndNotFirstTurn()
            {
                player1Hand = new CardCollection(
                    Card(8, Clubs),
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs)
                );
                var discardCard = deck.Get(10, Hearts);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(new PlayContext(sut.GetPlayer(1), Card(8, Clubs)));
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(1), Hearts));

                var actual = sut.Take(new TakeContext(sut.GetPlayer(2)));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
                sut.GameState.PreviousTurnResult.TakenCard.Should().Be(Card(1, Hearts));
            }

            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer1()
            {
                var discardCard = deck.Get(10, Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(new TakeContext(sut.GetPlayer(1)));

                var actual = sut.GameState.CurrentTable;
                actual.Players[0].Hand.Cards.Should().Contain(new Card(1, Hearts));
                actual.StockPile.Cards.Should().BeEmpty();
            }

            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer2()
            {
                var discardCard = deck.Get(1, Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts),
                    deck.Get(2, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(new PlayContext(sut.GetPlayer(1), player1Hand.Cards.First()));

                sut.Take(new TakeContext(sut.GetPlayer(2)));

                var actual = sut.GameState.CurrentTable;
                actual.Players[1].Hand.Cards.Should().Contain(new Card(1, Hearts));
                actual.StockPile.Cards.Should().Equal(deck.Get(2, Hearts));
            }
            
            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer3()
            {
                var discardCard = Card(2, Hearts);
                player3Hand = new CardCollection(
                    deck.Get(7, Spades),
                    deck.Get(3, Spades),
                    deck.Get(4, Spades),
                    deck.Get(5, Spades),
                    deck.Get(6, Spades)
                );

                var stockPile = new CardCollection(
                    Card(3, Hearts),
                    Card(4, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(new PlayContext(sut.GetPlayer(1), Card(2, Clubs)));
                sut.Play(new PlayContext(sut.GetPlayer(2), Card(2, Diamonds)));

                sut.Take(new TakeContext(sut.GetPlayer(3)));

                var actual = sut.GameState.CurrentTable;
                actual.Players[2].Hand.Cards.Should().Contain(Card(3, Hearts));
                actual.StockPile.Cards.Should().Equal(Card(4, Hearts));
            }

            [Fact]
            public void MoveDiscardCardsToStockPileWhenStockPileExhausted()
            {
                var discardPile = new DiscardPile( new CardCollection(
                    deck.Get(13, Spades),
                    deck.Get(2, Hearts),
                    deck.Get(3, Hearts),
                    deck.Get(1, Hearts)
                ));
                discardPile.TurnUpTopCard();
                var stockPile = new StockPile(new CardCollection(
                    deck.Get(1, Clubs)
                ));

                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;

                var currentTurn = new CurrentTurn(1, player1, Action.Take);
                
                var sut = new InProgressGameBuilder()
                    .WithCurrentPlayer(player1)
                    .WithPlayer(player1)
                    .WithPlayer(player2)
                    .WithPlayer(player3)
                    .WithDiscardPile(discardPile)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .Build();
                
                sut.Take(new TakeContext(sut.GetPlayer(1)));

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(deck.Get(13, Spades));
                sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(
                    deck.Get(2, Hearts),
                    deck.Get(3, Hearts),
                    deck.Get(1, Hearts)
                );

                var actualCard = sut.GameState.Events.ElementAt(1);
                actualCard.Should().BeOfType(typeof(CardMoved));
                var domainEvent = actualCard as CardMoved;
                domainEvent.Card.Should().Be(deck.Get(2, Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
                
                actualCard = sut.GameState.Events.ElementAt(2);
                actualCard.Should().BeOfType(typeof(CardMoved));
                domainEvent = actualCard as CardMoved;
                domainEvent.Card.Should().Be(deck.Get(3, Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
                
                actualCard = sut.GameState.Events.ElementAt(3);
                actualCard.Should().BeOfType(typeof(CardMoved));
                domainEvent = actualCard as CardMoved;
                domainEvent.Card.Should().Be(deck.Get(1, Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
            }
            
            [Fact]
            public void ShuffleStockPileWhenMovingDiscardCardsToStockPile()
            {
                var expectedDiscardPileRestOfCards = new[]
                {
                    deck.Get(5, Hearts),
                    deck.Get(6, Hearts),
                    deck.Get(4, Hearts)
                };
                var discardPile = new DiscardPile(new CardCollection(
                    expectedDiscardPileRestOfCards
                ));
                discardPile.AddCard(deck.Get(7, Spades));
                var expectedShuffledStockPile = new[]
                {
                    deck.Get(12, Spades),
                    deck.Get(13, Clubs),
                    deck.Get(11, Diamonds)
                };

                var shufflerMock = new Mock<IShuffler>();
                shufflerMock.Setup(x => x.Shuffle(It.Is<CardCollection>(y => y.Cards.ToArray().SequenceEqual(expectedDiscardPileRestOfCards))))
                    .Returns(new CardCollection(expectedShuffledStockPile
                ));
                var stockPile = new StockPile(new CardCollection(
                    deck.Get(1, Clubs)
                ));

                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;

                var currentTurn = new CurrentTurn(1, player1, Action.Take);
                
                var sut = new InProgressGameBuilder()
                    .WithShuffler(shufflerMock.Object)
                    .WithCurrentPlayer(player1)
                    .WithPlayer(player1)
                    .WithPlayer(player2)
                    .WithPlayer(player3)
                    .WithDiscardPile(discardPile)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .Build();
                
                sut.Take(new TakeContext(sut.GetPlayer(1)));

                shufflerMock.Verify(x => x.Shuffle(It.Is<CardCollection>(y => y.Cards.ToArray().SequenceEqual(expectedDiscardPileRestOfCards))));
                sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(
                    expectedShuffledStockPile
                );
                sut.GameState.Events.Last().Should().BeOfType<Shuffled>();
                var domainEvent = sut.GameState.Events.Last() as Shuffled;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Target.Should().Be(CardMoveSources.StockPile);
                domainEvent.StartCards.Cards.Should().Equal(expectedDiscardPileRestOfCards);
                domainEvent.EndCards.Cards.Should().Equal(expectedShuffledStockPile);
            }

            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer1()
            {
                var discardCard = deck.Get(13, Spades);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(2)
                    .Build();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(1)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }
            
            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer2()
            {
                var discardCard = deck.Get(13, Spades);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(1)
                    .Build();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(2)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer3()
            {
                var discardCard = deck.Get(13, Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(1)
                    .Build();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(3)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }
            
            [Fact]
            public void ReturnInvalidTakeWhenNextActionIsNotTake()
            {
                var discardCard = deck.Get(10, Clubs);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(1)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void ReturnInvalidTakeWhenFirstTurnAndCardToMatchIsEight()
            {
                var discardCard = deck.Get(8, Clubs);
                var stockPile = new CardCollection(
                    deck.Get(1, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(1)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateTakenEvent()
            {
                var discardCard = deck.Get(10, Hearts);
                var stockPile = new CardCollection(
                    deck.Get(9, Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(new TakeContext(sut.GetPlayer(1)));

                var actualEvent = sut.GameState.Events.LastSkip(1);
                actualEvent.Should().BeOfType(typeof(Taken));
                var domainEvent = actualEvent as Taken;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
                domainEvent.Card.Should().Be(stockPile.First());
            }
        }

        public class PlayWhenWonShould
        {
            private readonly Player player1;
            private readonly Player player2;
            private readonly Player player3;
            private readonly StockPile stockPile;

            public PlayWhenWonShould()
            {
                var player1Hand = new CardCollection(Card(1, Clubs));
                var player2Hand = new CardCollection(Card(2, Diamonds));
                var player3Hand = new CardCollection(Card(3, Spades));

                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                player3 = sampleData.Player3;
                player3.Hand = player3Hand;

                stockPile = new StockPile(new CardCollection());
            }

            [Fact]
            public void ReturnTrue_WhenPlayer1WonAfterPlay()
            {
                var discardCard = Card(1, Hearts);
                var currentTurn = new CurrentTurn(7, player1, Action.Play);
                var sut = new InProgressGameBuilder()
                    .WithPlayer(player1)
                    .WithPlayer(player2)
                    .WithPlayer(player3)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player1)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), player1.Hand.Cards.First()));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(7);
                actual.NextAction.Should().Be(Action.Won);

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeTrue();
                actualPreviousTurnResult.Winner.Number.Should().Be(1);
            }

            [Fact]
            public void ReturnTrue_WhenPlayer2WonAfterPlay()
            {
                var discardCard = Card(2, Hearts);
                var currentTurn = new CurrentTurn(8, player2, Action.Play);
                var sut = new InProgressGameBuilder()
                    .WithPlayer(player1)
                    .WithPlayer(player2)
                    .WithPlayer(player3)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player2)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(2), player2.Hand.Cards.First()));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(8);
                actual.NextAction.Should().Be(Action.Won);

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeTrue();
                actualPreviousTurnResult.Winner.Number.Should().Be(2);
            }

            [Fact]
            public void ReturnTrue_WhenPlayer3WonAfterPlay()
            {
                var discardCard = Card(3, Hearts);
                var currentTurn = new CurrentTurn(9, player3, Action.Play);
                var sut = new InProgressGameBuilder()
                    .WithPlayer(player1)
                    .WithPlayer(player2)
                    .WithPlayer(player3)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player3)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(3), player3.Hand.Cards.First()));

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(9);
                actual.NextAction.Should().Be(Action.Won);

                var actualPreviousTurnResult = sut.GameState.PreviousTurnResult;
                actualPreviousTurnResult.HasWinner.Should().BeTrue();
                actualPreviousTurnResult.Winner.Number.Should().Be(3);
            }

            [Fact]
            public void CreateRoundWonEvent()
            {
                var discardCard = Card(1, Hearts);
                var currentTurn = new CurrentTurn(7, player1, Action.Play);
                var sut = new InProgressGameBuilder()
                    .WithPlayer(player1)
                    .WithPlayer(player2)
                    .WithPlayer(player3)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player1)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), player1.Hand.Cards.First()));

                sut.GameState.Events.Last().Should().BeOfType(typeof(RoundWon));
                var domainEvent = sut.GameState.Events.Last() as RoundWon;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
            }
        }
    }
}
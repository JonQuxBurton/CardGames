using System.Linq;
using FluentAssertions;
using Moq;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;
using Xunit;

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
                var rules = new Rules(7);
                var sut = new Game(rules, new DummyShuffler(), new CardCollectionBuilder().Build(), new [] { sampleData.Player1, sampleData.Player2, sampleData.Player3 });

                sut.GameState.CurrentTurn.Should().BeNull();
                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.New);
                sut.GameState.Events.Should().BeEmpty();
            }
        }

        //public class InitialiseShould
        //{
        //    private Game sut;

        //    public InitialiseShould()
        //    {
        //        var sampleData = new SampleData();
        //        var rules = new Rules(7);
        //        sut = new Game(rules, new DummyShuffler(), new Dealer(rules), new CardCollectionBuilder().Build(), new [] { sampleData.Player1, sampleData.Player2 });
        //    }

        //    [Fact]
        //    public void CreateInitialisedEvent()
        //    {
        //        var gameState = new GameState(GamePhase.New);

        //        sut.Initialise(gameState);

        //        sut.Events.Last().Number.Should().Be(1);
        //        sut.Events.Last().Should().BeOfType(typeof(Initialised));
        //    }

        //    [Fact]
        //    public void SetGameStateWhenGamePhaseIsNew()
        //    {
        //        var gameState = new GameState(GamePhase.New);
                
        //        sut.Initialise(gameState);

        //        sut.GameState.CurrentGamePhase.Should().Be(GamePhase.New);
        //        sut.GameState.CurrentTable.Should().BeNull();
        //        sut.GameState.StartingPlayer.Should().BeNull();
        //        sut.GameState.CurrentTurn.Should().BeNull();
        //    }
            
        //    [Theory]
        //    [InlineData(1)]
        //    [InlineData(2)]
        //    [InlineData(3)]
        //    public void SetGameStateWhenGamePhaseIsReadyToDeal(int expectedStartingPlayer)
        //    {
        //        var gameState = new GameState(GamePhase.ReadyToDeal, expectedStartingPlayer);
                
        //        sut.Initialise(gameState);

        //        sut.GameState.CurrentGamePhase.Should().Be(GamePhase.ReadyToDeal);
        //        sut.GameState.CurrentTable.Should().BeNull();
        //        sut.GameState.StartingPlayer.Should().Be(expectedStartingPlayer);
        //        sut.GameState.CurrentTurn.Should().BeNull();
        //    }

        //    [Theory]
        //    [InlineData(1)]
        //    [InlineData(2)]
        //    [InlineData(3)]
        //    public void SetGameStateWhenGamePhaseIsInGame(int expectedStartingPlayerNumber)
        //    {
        //        var player1Hand = new CardCollection(new Card(1, Suit.Clubs));
        //        var player2Hand = new CardCollection(new Card(1, Suit.Diamonds));
        //        var player3Hand = new CardCollection(new Card(10, Suit.Spades));
        //        var stockPile = new StockPile(new CardCollection(new Card(1, Suit.Hearts), new Card(2, Suit.Hearts)));
        //        var discardPile = new DiscardPile(new[]
        //        {
        //            new Card(1, Suit.Spades),
        //            new Card(2, Suit.Spades)
        //        });
        //        var sampleData = new SampleData();
        //        var player1 = sampleData.Player1;
        //        player1.Hand = player1Hand;
        //        var player2 = sampleData.Player2;
        //        player2.Hand = player2Hand;
        //        var player3 = sampleData.Player3;
        //        player3.Hand = player3Hand;
        //        var expectedStartingPlayer = sampleData.GetPlayer(expectedStartingPlayerNumber);
        //        var expectedTurn = new CurrentTurn(1, expectedStartingPlayer, new Card[0],
        //            false, null, Action.Play, null);
        //        sut = new InProgressGameBuilder()
        //            .WithStartingPlayer(expectedStartingPlayer.Number)
        //            .WithPlayer(player1)
        //            .WithPlayer(player2)
        //            .WithPlayer(player3)
        //            .WithStockPile(stockPile)
        //            .WithDiscardPile(discardPile)
        //            .WithCurrentTurn(expectedTurn)
        //            .Build();
                
        //        sut.GameState.CurrentGamePhase.Should().Be(GamePhase.InGame);
        //        sut.GameState.StartingPlayer.Should().Be(expectedStartingPlayer.Number);
        //        sut.GameState.CurrentTable.Players[0].Hand.Should().Be(player1Hand);
        //        sut.GameState.CurrentTable.Players[1].Hand.Should().Be(player2Hand);
        //        sut.GameState.CurrentTable.Players[2].Hand.Should().Be(player3Hand);
        //        sut.GameState.CurrentTable.StockPile.Should().Be(stockPile);
        //        sut.GameState.CurrentTable.DiscardPile.Should().Be(discardPile);
        //        sut.GameState.CurrentTurn.Should().Be(expectedTurn);
        //    }
        //}

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
                var shuffler = new DummyShuffler();
                var rules = new Rules(7);
                var deck = new DeckBuilder().Build();
                sut = new Game(rules, shuffler, deck, new[] { player1, player2, player3 });
            }

            [Fact]
            public void CreateStartingPlayerChosenEvent()
            {
                sut.ChooseStartingPlayer(player2);

                sut.GameState.Events.Last().Number.Should().Be(1);
                sut.GameState.Events.Last().Should().BeOfType(typeof(StartingPlayerChosen));
                var startingPlayerChosenEvent = sut.GameState.Events.Last() as StartingPlayerChosen;
                startingPlayerChosenEvent.Player.Number.Should().Be(2);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public void SetStartingPlayer(int startingPlayer)
            {
                var sampleData = new SampleData();
                sut.ChooseStartingPlayer(sampleData.GetPlayer(startingPlayer));
                
                sut.GameState.CurrentPlayer.Number.Should().Be(startingPlayer);
            }

            [Fact]
            public void SetGamePhaseToReadyToDeal1()
            {
                sut.ChooseStartingPlayer(player1);
                
                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.ReadyToDeal);
            }
        }

        public class GetCardShould
        {
            private readonly Game sut;

            public GetCardShould()
            {
                var player1Hand = new CardCollection(
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(7, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds),
                    new Card(6, Suit.Diamonds),
                    new Card(7, Suit.Diamonds)
                );
                var player3Hand = new CardCollection(
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Spades),
                    new Card(5, Suit.Spades),
                    new Card(6, Suit.Spades),
                    new Card(7, Suit.Spades)
                );
                var stockPile = new CardCollection(new Card(1, Suit.Hearts));
                var discardCard = new Card(2, Suit.Hearts);

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
                var actual = sut.GetCard(1, Suit.Spades);

                actual.Should().Be(new Card(1, Suit.Spades));
            }
            
            [Fact]
            public void ReturnCardFromDiscardPile()
            {
                var actual = sut.GetCard(2, Suit.Hearts);

                actual.Should().Be(new Card(2, Suit.Hearts));
            }
            
            [Fact]
            public void ReturnCardFromPlayer1Hand()
            {
                var actual = sut.GetCard(7, Suit.Clubs);

                actual.Should().Be(new Card(7, Suit.Clubs));
            }
            
            [Fact]
            public void ReturnCardFromPlayer2Hand()
            {
                var actual = sut.GetCard(4, Suit.Diamonds);

                actual.Should().Be(new Card(4, Suit.Diamonds));
            }
            
            [Fact]
            public void ReturnCardFromPlayer3Hand()
            {
                var actual = sut.GetCard(2, Suit.Spades);

                actual.Should().Be(new Card(2, Suit.Spades));
            }
            
            [Fact]
            public void ReturnNullWhenCardNotFound()
            {
                var actual = sut.GetCard(13, Suit.Spades);

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
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(5, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(7, Suit.Clubs)
                );
                player2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds),
                    new Card(3, Suit.Diamonds),
                    new Card(4, Suit.Diamonds),
                    new Card(5, Suit.Diamonds),
                    new Card(6, Suit.Diamonds),
                    new Card(7, Suit.Diamonds)
                );
                player3Hand = new CardCollection(
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades),
                    new Card(3, Suit.Spades),
                    new Card(4, Suit.Spades),
                    new Card(5, Suit.Spades),
                    new Card(6, Suit.Spades),
                    new Card(7, Suit.Spades)
                );
                stockPile = new CardCollection(new Card(1, Suit.Hearts));
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
            public void CreateCardMovedEvents()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                CardMoved domainEvent;

                for (int i = 0; i < 7; i++)
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

                sut.GameState.Events.ElementAt(22).Should().BeOfType(typeof(CardMoved));
                domainEvent = sut.GameState.Events.ElementAt(22) as CardMoved;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Card.Should().Be(discardCard);
                domainEvent.FromSource.Should().Be(CardMoveSources.StockPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.DiscardPile);
            }
            
            [Fact]
            public void CreateDealCompletedEvent()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.Events.Last().Should().BeOfType(typeof(DealCompleted));
            }

            [Fact]
            public void ShuffleCards()
            {
                var discardCard = new Card(2, Suit.Hearts);
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
                var discardCard = new Card(2, Suit.Hearts);
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
                var discardCard = new Card(2, Suit.Hearts);
                var sut = CreateSut(expectedStartingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentPlayer.Number.Should().Be(expectedStartingPlayer);
            }
            
            [Fact]
            public void AddHandToPlayer1()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            }

            [Fact]
            public void AddHandToPlayer2()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                var actual = sut.GameState.CurrentTable;
                actual.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            }
            
            [Fact]
            public void AddHandToPlayer3()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                var actual = sut.GameState.CurrentTable;
                actual.Players[2].Hand.Cards.Should().Equal(player3Hand.Cards);
            }

            [Fact]
            public void SetupDiscardPile()
            {
                var expectedDiscardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, expectedDiscardCard);

                sut.Deal();

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(expectedDiscardCard);
            }

            [Fact]
            public void SetupStockPile()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(stockPile.Cards);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer1()
            {
                var expectedValidPlaysForPlayer1 = new CardCollection(new Card(2, Suit.Clubs));
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                //var actual = sut.GameState.CurrentTurn;
                //actual.TurnNumber.Should().Be(1);
                //actual.HasWinner.Should().BeFalse();
                //actual.Winner.Should().BeNull();
                //actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer1.Cards);
                //actual.NextAction.Should().Be(Action.Play);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer1.Cards);
                actual.NextAction.Should().Be(Action.Play);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer2()
            {
                var expectedValidPlaysForPlayer2 = new CardCollection(new Card(2, Suit.Diamonds));
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 2;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                //var actual = sut.GameState.CurrentTurn;
                //actual.TurnNumber.Should().Be(1);
                //actual.HasWinner.Should().BeFalse();
                //actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer2.Cards);
                //actual.NextAction.Should().Be(Action.Play);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.HasWinner.Should().BeFalse();
                actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer2.Cards);
                actual.NextAction.Should().Be(Action.Play);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer3()
            {
                var expectedValidPlaysForPlayer3 = new CardCollection(new Card(2, Suit.Spades));
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 3;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                //var actual = sut.GameState.CurrentTurn;
                //actual.TurnNumber.Should().Be(1);
                //actual.HasWinner.Should().BeFalse();
                //actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer3.Cards);
                //actual.NextAction.Should().Be(Action.Play);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.HasWinner.Should().BeFalse();
                actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer3.Cards);
                actual.NextAction.Should().Be(Action.Play);
            }

            [Fact]
            public void CreateFirstTurnWithNextActionTake()
            {
                var discardCard = new Card(10, Suit.Spades);
                var startingPlayer = 1;
                var sut = new ReadyToDealGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(startingPlayer)
                    .Build();

                sut.Deal();

                //var actual = sut.GameState.CurrentTurn;
                //actual.TurnNumber.Should().Be(1);
                //actual.ValidPlays.Should().BeEmpty();
                //actual.NextAction.Should().Be(Action.Take);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
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
                    deck.Get(2, Suit.Clubs),
                    deck.Get(4, Suit.Spades)
                );
                player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds)
                );
                player3Hand = new CardCollection(
                    deck.Get(1, Suit.Spades),
                    deck.Get(2, Suit.Spades),
                    deck.Get(3, Suit.Spades)
                );
            }

            [Fact]
            public void ReturnTrueWhenMatchingSuit()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(1, playedCard);

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnTrueWhenMatchingRank()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(10, Suit.Hearts);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(1, playedCard);

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnTrueWhenRank8()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var playedCard = deck.Get(8, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(1, playedCard);

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void AddCardToDiscardPile()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard);
            }

            [Fact]
            public void RemoveCardFromPlayersHand()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().NotContain(playedCard);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(1, Suit.Diamonds));
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().NotContain(playedCard);
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard);
            }

            [Fact]
            public void CreateNewTurnWithNextActionTake()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                var player2HandWithNoPlays = new CardCollection(
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds),
                    deck.Get(4, Suit.Diamonds)
                );

                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2HandWithNoPlays)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Take);
            }

            [Fact]
            public void CreateNewTurnAfterPlayer2TurnWithValidPlay()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                var playedCard2 = deck.Get(1, Suit.Diamonds);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);
                sut.Play(2, playedCard2);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(3);
                actual.PlayerToPlay.Number.Should().Be(3);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(1, Suit.Spades));
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.CurrentTable.Players[1].Hand.Cards.Should().NotContain(playedCard2);
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard2);
            }
            
            [Fact]
            public void CreateNewTurnAfterPlayer3TurnWithValidPlay()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                var playedCard2 = deck.Get(1, Suit.Diamonds);
                var playedCard3 = deck.Get(1, Suit.Spades);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);
                sut.Play(2, playedCard2);
                sut.Play(3, playedCard3);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(4);
                actual.PlayerToPlay.Number.Should().Be(1);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(4, Suit.Spades));
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.CurrentTable.Players[2].Hand.Cards.Should().NotContain(playedCard3);
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard3);
            }

            [Fact]
            public void CreatePlayedEvent()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);

                sut.GameState.Events.Last().Should().BeOfType(typeof(Played));
                var played = sut.GameState.Events.Last() as Played;
                if (played == null) Assert.NotNull(played);
                played.PlayerNumber.Should().Be(1);
                played.Card.Should().Be(playedCard);
            }
        }

        public class PlayWhenPlayingEightShould
        {
            private readonly CardCollection deck;
            private Game sut;

            public PlayWhenPlayingEightShould()
            {
                deck = new CardCollectionBuilder().Build();

                var player1Hand = new CardCollection(
                    deck.Get(1, Suit.Clubs),
                    deck.Get(8, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds)
                );
                var player3Hand = new CardCollection(
                    deck.Get(1, Suit.Spades),
                    deck.Get(2, Suit.Spades)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(deck.Get(1, Suit.Spades))
                    .Build();
            }

            [Fact]
            public void ReturnTrue()
            {
                var actual = sut.Play(1, deck.Get(8, Suit.Clubs));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void CreateCrazyEightTurn()
            {
                var cardToPlay = deck.Get(8, Suit.Clubs);

                sut.Play(1, cardToPlay);

                //var actual = sut.GameState.CurrentTurn;
                //actual.TurnNumber.Should().Be(1);
                //actual.PlayerToPlay.Number.Should().Be(1);
                //actual.NextAction.Should().Be(Action.SelectSuit);
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
                    deck.Get(1, Suit.Clubs),
                    deck.Get(8, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    deck.Get(8, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds)
                );
                var player3Hand = new CardCollection(
                    deck.Get(8, Suit.Spades),
                    deck.Get(2, Suit.Spades)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(deck.Get(1, Suit.Spades))
                    .Build();

                sut.Play(1, deck.Get(1, Suit.Clubs));

                sut.Play(2, deck.Get(8, Suit.Diamonds));
                
                //var actual = sut.GameState.CurrentTurn;
                //actual.TurnNumber.Should().Be(2);
                //actual.PlayerToPlay.Number.Should().Be(2);
                //actual.NextAction.Should().Be(Action.SelectSuit);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.NextAction.Should().Be(Action.SelectSuit);
                
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(deck.Get(8, Suit.Diamonds));
            }            
            
            [Fact]
            public void CreateCrazyEightTurnWhenTurn3()
            {
                var player1Hand = new CardCollection(
                    deck.Get(1, Suit.Clubs),
                    deck.Get(8, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds)
                );
                var player3Hand = new CardCollection(
                    deck.Get(8, Suit.Spades),
                    deck.Get(2, Suit.Spades)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(deck.Get(1, Suit.Spades))
                    .Build();

                sut.Play(1, deck.Get(1, Suit.Clubs));
                sut.Play(2, deck.Get(1, Suit.Diamonds));

                sut.Play(3, deck.Get(8, Suit.Spades));
                
                //var actual = sut.GameState.CurrentTurn;
                //actual.TurnNumber.Should().Be(3);
                //actual.PlayerToPlay.Number.Should().Be(3);
                //actual.NextAction.Should().Be(Action.SelectSuit);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(3);
                actual.PlayerToPlay.Number.Should().Be(3);
                actual.NextAction.Should().Be(Action.SelectSuit);
                
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(deck.Get(8, Suit.Spades));
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

                originalDiscardCard = deck.Get(10, Suit.Hearts);
                var player1Hand = new CardCollection(
                    deck.Get(1, Suit.Clubs),
                    deck.Get(2, Suit.Clubs),
                    deck.Get(7, Suit.Hearts)
                );
                var player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(7, Suit.Diamonds),
                    deck.Get(10, Suit.Diamonds)
                );
                var player3Hand = new CardCollection(
                    deck.Get(1, Suit.Spades),
                    deck.Get(7, Suit.Spades),
                    deck.Get(10, Suit.Spades)
                );
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                playedCard = deck.Get(1, Suit.Clubs);

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
                var actual = sut.Play(1, deck.Get(1, Suit.Hearts));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer2Hand()
            {
                sut.Play(1, deck.Get(7, Suit.Hearts));
                var actual = sut.Play(2, deck.Get(1, Suit.Hearts));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer3Hand()
            {
                sut.Play(1, deck.Get(7, Suit.Hearts));
                sut.Play(2, deck.Get(7, Suit.Diamonds));
                var actual = sut.Play(3, deck.Get(1, Suit.Hearts));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs1AndIsNotCurrentPlayer()
            {
                sut.Play(1, deck.Get(7, Suit.Hearts));
                var actual = sut.Play(1, deck.Get(7, Suit.Diamonds));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs2AndIsNotCurrentPlayer()
            {
                var actual = sut.Play(2, deck.Get(10, Suit.Diamonds));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs3AndIsNotCurrentPlayer()
            {
                var actual = sut.Play(3, deck.Get(10, Suit.Spades));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayIsInvalid()
            {
                var actual = sut.Play(1, playedCard);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void NotAddCardToDiscardPile()
            {
                sut.Play(1, playedCard);

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(originalDiscardCard);
            }

            [Fact]
            public void NotRemoveCardFromPlayersHand()
            {
                sut.Play(1, playedCard);

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Contain(playedCard);
            }

            [Fact]
            public void NotCreateNewTurn()
            {
                sut.Play(1, playedCard);

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(1);
            }
        }

        public class SelectSuitShould
        {
            private readonly Game sut;

            public SelectSuitShould()
            {
                var deck = new CardCollectionBuilder().Build();

                var player1Hand = new CardCollection(
                    deck.Get(1, Suit.Clubs),
                    deck.Get(8, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    deck.Get(10, Suit.Diamonds),
                    deck.Get(11, Suit.Diamonds)
                );

                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(deck.Get(1, Suit.Spades))
                    .Build();
                sut.Play(1, deck.Get(8, Suit.Clubs));
            }

            [Fact]
            public void AddSelectedSuitToTurn()
            {
                sut.SelectSuit(1, Suit.Diamonds);

                var actualCurrentTurn = sut.GameState.CurrentTurn;
                actualCurrentTurn.TurnNumber.Should().Be(2);
                actualCurrentTurn.PlayerToPlay.Number.Should().Be(2);
                actualCurrentTurn.ValidPlays.Should().Equal(new Card(10, Suit.Diamonds), new Card(11, Suit.Diamonds));
                actualCurrentTurn.NextAction.Should().Be(Action.Play);
                actualCurrentTurn.SelectedSuit.Should().Be(Suit.Diamonds);
            }

            [Fact]
            public void KeepSelectedSuitOnTurnAfterTake()
            {
                sut.SelectSuit(1, Suit.Diamonds);

                sut.Take(2);

                sut.GameState.CurrentTurn.SelectedSuit.Should().Be(Suit.Diamonds);
            }

            [Fact]
            public void ReturnTrueWhenTakeIsValid()
            {
                var actual = sut.SelectSuit(1, Suit.Diamonds);

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnFalseWhenNotPlayersTurn()
            {
                var actual = sut.SelectSuit(2, Suit.Diamonds);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void CreateSuitSelectedEvent()
            {
                sut.SelectSuit(1, Suit.Diamonds);

                sut.GameState.Events.Last().Should().BeOfType(typeof(SuitSelected));
                var domainEvent = sut.GameState.Events.Last() as SuitSelected;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
                domainEvent.Suit.Should().Be(Suit.Diamonds);
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
                    new Card(1, Suit.Clubs),
                    new Card(8, Suit.Clubs)
                );
                player2Hand = new CardCollection
                (
                    new Card(1, Suit.Diamonds),
                    new Card(8, Suit.Diamonds)
                );
                player3Hand = new CardCollection
                (
                    new Card(1, Suit.Spades),
                    new Card(8, Suit.Spades)
                );

                discardCard = new Card(1, Suit.Hearts);
            }

            [Fact]
            public void ReturnTrueForPlayMatchingSelectedSuit()
            {
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, new Card(8, Suit.Clubs));
                sut.SelectSuit(1, Suit.Diamonds);

                var actual = sut.Play(2, new Card(1, Suit.Diamonds));
                actual.IsSuccess.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnTrueForPlayMatchingSelectedSuitAfterPlayer2()
            {
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(2)
                    .Build();

                sut.Play(2, new Card(8, Suit.Diamonds));
                sut.SelectSuit(2, Suit.Spades);

                var actual = sut.Play(3, new Card(1, Suit.Spades));
                actual.IsSuccess.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnTrueForPlayMatchingSelectedSuitAfterPlayer3()
            {
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(3)
                    .Build();

                sut.Play(3, new Card(8, Suit.Spades));
                sut.SelectSuit(3, Suit.Clubs);

                var actual = sut.Play(1, new Card(1, Suit.Clubs));
                actual.IsSuccess.Should().BeTrue();
            }
        }

        public class TakeShould
        {
            private readonly CardCollection deck;
            private CardCollection player1Hand;
            private CardCollection player2Hand;
            private readonly CardCollection player3Hand;

            public TakeShould()
            {
                deck = new CardCollectionBuilder().Build();
                player1Hand = new CardCollection(
                    deck.Get(1, Suit.Clubs),
                    deck.Get(2, Suit.Clubs),
                    deck.Get(3, Suit.Clubs)
                );
                player2Hand = new CardCollection(
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds),
                    deck.Get(10, Suit.Diamonds)
                );
                player3Hand = new CardCollection(
                    deck.Get(4, Suit.Spades),
                    deck.Get(5, Suit.Spades),
                    deck.Get(6, Suit.Spades)
                );
            }

            [Fact]
            public void CreatesNewTurn()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(9, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(1);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(10, Suit.Diamonds));
                actual.NextAction.Should().Be(Action.Play);
                
                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            }

            [Fact]
            public void ReturnCardTaken()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(1);

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
                actual.Card.Should().Be(new Card(1, Suit.Hearts));
            }

            [Fact]
            public void ReturnCardTakenWhenCardToMatchIsEightAndNotFirstTurn()
            {
                player1Hand = new CardCollection(
                    deck.Get(8, Suit.Clubs),
                    deck.Get(2, Suit.Clubs),
                    deck.Get(3, Suit.Clubs)
                );
                player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds)
                );

                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(1, new Card(8, Suit.Clubs));
                sut.SelectSuit(1, Suit.Hearts);

                var actual = sut.Take(2);

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
                actual.Card.Should().Be(new Card(1, Suit.Hearts));
            }

            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer1()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(1);

                var actual = sut.GameState.CurrentTable;
                actual.Players[0].Hand.Cards.Should().Contain(new Card(1, Suit.Hearts));
                actual.StockPile.Cards.Should().BeEmpty();
            }

            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer2()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts),
                    deck.Get(2, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(1, player1Hand.Cards.First());

                sut.Take(2);

                var actual = sut.GameState.CurrentTable;
                actual.Players[1].Hand.Cards.Should().Contain(new Card(1, Suit.Hearts));
                actual.StockPile.Cards.Should().Equal(deck.Get(2, Suit.Hearts));
            }
            
            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer3()
            {
                var discardCard = deck.Get(2, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(3, Suit.Hearts),
                    deck.Get(4, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(1, new Card(2, Suit.Clubs));
                sut.Play(2, new Card(2, Suit.Diamonds));

                sut.Take(3);

                var actual = sut.GameState.CurrentTable;
                actual.Players[2].Hand.Cards.Should().Contain(new Card(3, Suit.Hearts));
                actual.StockPile.Cards.Should().Equal(deck.Get(4, Suit.Hearts));
            }

            [Fact]
            public void MoveDiscardCardsToStockPileWhenStockPileExhausted()
            {
                var discardPile = new DiscardPile( new CardCollection(
                    deck.Get(13, Suit.Spades),
                    deck.Get(2, Suit.Hearts),
                    deck.Get(3, Suit.Hearts),
                    deck.Get(1, Suit.Hearts)
                ));
                discardPile.TurnUpTopCard();
                var stockPile = new StockPile(new CardCollection(
                    deck.Get(1, Suit.Clubs)
                ));

                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;

                var currentTurn = new CurrentTurn(1, player1, new Card[0], false, null, Action.Take, null);
                
                var sut = new InProgressGameBuilder()
                    .WithCurrentPlayer(player1)
                    .WithPlayer(player1)
                    .WithPlayer(player2)
                    .WithPlayer(player3)
                    .WithDiscardPile(discardPile)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .Build();
                
                sut.Take(1);

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(deck.Get(13, Suit.Spades));
                sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(
                    deck.Get(2, Suit.Hearts),
                    deck.Get(3, Suit.Hearts),
                    deck.Get(1, Suit.Hearts)
                );

                var actualCard = sut.GameState.Events.ElementAt(1);
                actualCard.Should().BeOfType(typeof(CardMoved));
                var domainEvent = actualCard as CardMoved;
                domainEvent.Card.Should().Be(deck.Get(2, Suit.Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
                
                actualCard = sut.GameState.Events.ElementAt(2);
                actualCard.Should().BeOfType(typeof(CardMoved));
                domainEvent = actualCard as CardMoved;
                domainEvent.Card.Should().Be(deck.Get(3, Suit.Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
                
                actualCard = sut.GameState.Events.ElementAt(3);
                actualCard.Should().BeOfType(typeof(CardMoved));
                domainEvent = actualCard as CardMoved;
                domainEvent.Card.Should().Be(deck.Get(1, Suit.Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
            }
            
            [Fact]
            public void ShuffleStockPileWhenMovingDiscardCardsToStockPile()
            {
                var expectedDiscardPileRestOfCards = new[]
                {
                    deck.Get(5, Suit.Hearts),
                    deck.Get(6, Suit.Hearts),
                    deck.Get(4, Suit.Hearts)
                };
                var discardPile = new DiscardPile(new CardCollection(
                    expectedDiscardPileRestOfCards
                ));
                discardPile.AddCard(deck.Get(7, Suit.Spades));
                var expectedShuffledStockPile = new[]
                {
                    deck.Get(12, Suit.Spades),
                    deck.Get(13, Suit.Clubs),
                    deck.Get(11, Suit.Diamonds)
                };

                var shufflerMock = new Mock<IShuffler>();
                shufflerMock.Setup(x => x.Shuffle(It.Is<CardCollection>(y => y.Cards.ToArray().SequenceEqual(expectedDiscardPileRestOfCards))))
                    .Returns(new CardCollection(expectedShuffledStockPile
                ));
                var stockPile = new StockPile(new CardCollection(
                    deck.Get(1, Suit.Clubs)
                ));

                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;

                var currentTurn = new CurrentTurn(1, player1, new Card[0], false, null, Action.Take, null);
                
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
                
                sut.Take(1);

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

            //[Theory]
            //[InlineData(0)]
            //[InlineData(3)]
            //public void ReturnNotPlayersTurnWhenInvalidPlayerNumber(int playerNumber)
            //{
            //    var discardCard = deck.Get(10, Suit.Hearts);
            //    var stockPile = new CardCollection(
            //        deck.Get(1, Suit.Hearts)
            //    );
            //    var sut = new AtStartGameBuilder()
            //        .WithPlayer1Hand(player1Hand)
            //        .WithPlayer2Hand(player2Hand)
            //        .WithPlayer3Hand(player3Hand)
            //        .WithDiscardCard(discardCard)
            //        .WithStockPile(stockPile)
            //        .Build();

            //    var actual = sut.Take(playerNumber);

            //    actual.IsSuccess.Should().BeFalse();
            //    actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            //    actual.Card.Should().Be(null);
            //}

            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer1()
            {
                var discardCard = deck.Get(13, Suit.Spades);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(2)
                    .Build();

                var actual = sut.Take(1);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                actual.Card.Should().Be(null);
            }
            
            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer2()
            {
                var discardCard = deck.Get(13, Suit.Spades);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(1)
                    .Build();

                var actual = sut.Take(2);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                actual.Card.Should().Be(null);
            }

            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer3()
            {
                var discardCard = deck.Get(13, Suit.Spades);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(1)
                    .Build();

                var actual = sut.Take(3);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                actual.Card.Should().Be(null);
            }
            
            [Fact]
            public void ReturnInvalidTakeWhenNextActionIsNotTake()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(1);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
                actual.Card.Should().Be(null);
            }

            [Fact]
            public void ReturnInvalidTakeWhenFirstTurnAndCardToMatchIsEight()
            {
                var discardCard = deck.Get(8, Suit.Clubs);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(1);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
                actual.Card.Should().Be(null);
            }

            [Fact]
            public void CreateTakenEvent()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(9, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(1);

                var actualEvent = sut.GameState.Events.ElementAt(12);
                actualEvent.Should().BeOfType(typeof(Taken));
                var domainEvent = actualEvent as Taken;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
                domainEvent.Card.Should().Be(stockPile.First());
            }
        }

        public class PlayWhenWonShould
        {
            private readonly Card discardCard;
            private CardCollection player1Hand;
            private CardCollection player2Hand;
            private CardCollection player3Hand;
            private Game sut;

            public PlayWhenWonShould()
            {
                var sampleData = new SampleData();
                discardCard = new Card(13, Suit.Diamonds);
                var rules = new Rules(7);
                sut = new Game(rules, new DummyShuffler(), new CardCollectionBuilder().Build(), new[] { sampleData.Player1, sampleData.Player2, sampleData.Player3 });
            }

            [Fact]
            public void ReturnTrueWhenPlayer1WonAfterPlay()
            {
                player1Hand = new CardCollection(new Card(13, Suit.Clubs));
                player2Hand = new CardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs)
                    );
                sut = new AtStartGameBuilder()
                .WithPlayer1Hand(player1Hand)
                .WithPlayer2Hand(player2Hand)
                .WithDiscardCard(discardCard)
                .Build();

                sut.Play(1, player1Hand.Cards.First());

                //var actual = sut.GameState.CurrentTurn;
                //actual.HasWinner.Should().BeTrue();
                //actual.Winner.Number.Should().Be(1);
                //actual.TurnNumber.Should().Be(1);
                //actual.ValidPlays.Should().BeEmpty();
                //actual.NextAction.Should().Be(Action.Won);
                
                var actual = sut.GameState.CurrentTurn;
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(1);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }

            [Fact]
            public void ReturnTrueWhenWonOnTurn2()
            {
                player1Hand = new CardCollection(
                    new Card(1, Suit.Clubs)
                    );
                player2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds)
                    );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(new CardCollection
                    (
                        new Card(2, Suit.Clubs)
                    ))
                    .Build();
                sut.Take(1);
                sut.Play(2, player2Hand.Cards.First());

                //var actual = sut.GameState.CurrentTurn;
                //actual.HasWinner.Should().BeTrue();
                //actual.Winner.Number.Should().Be(2);
                //actual.TurnNumber.Should().Be(2);
                //actual.ValidPlays.Should().BeEmpty();
                //actual.NextAction.Should().Be(Action.Won);
                
                var actual = sut.GameState.CurrentTurn;
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(2);
                actual.TurnNumber.Should().Be(2);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }

            [Fact]
            public void ReturnTrueWhenWonOnTurn3()
            {
                player1Hand = new CardCollection(
                    new Card(13, Suit.Clubs),
                    new Card(1, Suit.Clubs)
                    );
                player2Hand = new CardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(3, Suit.Clubs)
                    );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(new CardCollection
                    (
                        new Card(1, Suit.Hearts)
                    ))
                    .Build();
                sut.Play(1, new Card(13, Suit.Clubs));
                sut.Play(2, new Card(2, Suit.Clubs));
                sut.Play(1, new Card(1, Suit.Clubs));

                //var actual = sut.GameState.CurrentTurn;
                //actual.HasWinner.Should().BeTrue();
                //actual.Winner.Number.Should().Be(1);
                //actual.TurnNumber.Should().Be(3);
                //actual.ValidPlays.Should().BeEmpty();
                //actual.NextAction.Should().Be(Action.Won);
                
                var actual = sut.GameState.CurrentTurn;
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(1);
                actual.TurnNumber.Should().Be(3);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }

            [Fact]
            public void ReturnTrueWhenPlayer2WonAfterPlay()
            {
                player1Hand = new CardCollection(
                    new Card(2, Suit.Clubs)
                );
                player2Hand = new CardCollection(new Card(13, Suit.Clubs));
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(2)
                    .Build();
                sut.Play(2, player2Hand.Cards.First());

                //var actual = sut.GameState.CurrentTurn;
                //actual.HasWinner.Should().BeTrue();
                //actual.Winner.Number.Should().Be(2);
                //actual.TurnNumber.Should().Be(1);
                //actual.ValidPlays.Should().BeEmpty();
                //actual.NextAction.Should().Be(Action.Won);
                
                var actual = sut.GameState.CurrentTurn;
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(2);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }

            [Fact]
            public void ReturnTrueWhenPlayer3WonAfterPlay()
            {
                player1Hand = new CardCollection(
                    new Card(1, Suit.Clubs)
                );
                player2Hand = new CardCollection(new Card(1, Suit.Diamonds));
                player3Hand = new CardCollection(new Card(13, Suit.Spades));
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(3)
                    .Build();
                sut.Play(3, player3Hand.Cards.First());

                //var actual = sut.GameState.CurrentTurn;
                //actual.HasWinner.Should().BeTrue();
                //actual.Winner.Number.Should().Be(3);
                //actual.TurnNumber.Should().Be(1);
                //actual.ValidPlays.Should().BeEmpty();
                //actual.NextAction.Should().Be(Action.Won);
                
                var actual = sut.GameState.CurrentTurn;
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(3);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }

            [Fact]
            public void CreateTakenEvent()
            {
                player1Hand = new CardCollection(new Card(13, Suit.Clubs));
                player2Hand = new CardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();
                sut.Play(1, player1Hand.Cards.First());

                sut.GameState.Events.Last().Should().BeOfType(typeof(RoundWon));
                var domainEvent = sut.GameState.Events.Last() as RoundWon;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
            }
        }

    }
}
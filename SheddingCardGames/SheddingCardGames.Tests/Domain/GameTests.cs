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
                if (startingPlayerChosenEvent == null) Assert.NotNull(startingPlayerChosenEvent);
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

        public class DealShould
        {
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private readonly CardCollection player3Hand;
            private readonly CardCollection stockPile;
            private Card discardCard;

            public DealShould()
            {
                discardCard = Card(2, Hearts);
                player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(5, Clubs)
                );
                player2Hand = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds)
                );
                player3Hand = new CardCollection(
                    Card(1, Spades),
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades)
                );
                stockPile = new CardCollection(Card(1, Hearts));
            }

            [Fact]
            public void ReturnTrue()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                var actual = sut.Deal();

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void CreateCardMovedEvents()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

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
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                sut.GameState.Events.Last().Should().BeOfType(typeof(DealCompleted));
            }

            [Fact]
            public void ShuffleCards()
            {
                var expectedDeck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand).Build();
                var shufflerMock = new Mock<IShuffler>();
                shufflerMock.Setup(x => x.Shuffle(It.Is<CardCollection>(y => y.Cards.SequenceEqual(expectedDeck.Cards) )))
                    .Returns(expectedDeck);
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .WithShuffler(shufflerMock.Object)
                    .BuildReadyToDealGame();

                sut.Deal();
                
                shufflerMock.VerifyAll();
            }
            
            [Fact]
            public void SetGameStateToInGame()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();
                
                sut.Deal();

                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.InGame);
            }
            
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            [InlineData(3)]
            public void PreserveStartingPlayer(int expectedStartingPlayer)
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(expectedStartingPlayer)
                    .BuildReadyToDealGame();

                sut.Deal();

                sut.GameState.CurrentTurn.PlayerToPlay.Number.Should().Be(expectedStartingPlayer);
            }
            
            [Fact]
            public void AddHandToPlayer1()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            }

            [Fact]
            public void AddHandToPlayer2()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                var actual = sut.GameState.CurrentTable;
                actual.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            }
            
            [Fact]
            public void AddHandToPlayer3()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                var actual = sut.GameState.CurrentTable;
                actual.Players[2].Hand.Cards.Should().Equal(player3Hand.Cards);
            }

            [Fact]
            public void SetupDiscardPile()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            }

            [Fact]
            public void SetupStockPile()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(stockPile.Cards);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer1()
            {
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer2()
            {
                var sut = new GameBuilder()
                    .WithStartingPlayer(2)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();
                
                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer3()
            {
                var sut = new GameBuilder()
                    .WithStartingPlayer(3)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(discardCard)
                    .BuildReadyToDealGame();

                sut.Deal();

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateFirstTurnWithNextActionTake()
            {
                discardCard = new Card(10, Spades);
                var sut = new GameBuilder()
                    .WithStartingPlayer(1)
                    .WithDiscardCard(discardCard)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithStockPile(stockPile)
                    .BuildReadyToDealGame();

                sut.Deal();

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.NextAction.Should().Be(Action.Take);
            }
        }

        public class PlayWhenValidShould
        {
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private readonly CardCollection player3Hand;
            private Game sut;

            public PlayWhenValidShould()
            {
                new CardCollectionBuilder().Build();

                player1Hand = new CardCollection(
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(10, Spades)
                );
                player2Hand = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds)
                );
                player3Hand = new CardCollection(
                    Card(1, Spades),
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades)
                );
            }

            [Fact]
            public void ReturnTrueWhenMatchingSuit()
            {
                var discardCard = Card(10, Clubs);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();
                
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnTrueWhenMatchingRank()
            {
                var discardCard = Card(10, Clubs);
                var playedCard = Card(10, Hearts);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void ReturnTrueWhenRank8()
            {
                var discardCard = Card(10, Hearts);
                var playedCard = Card(8, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }

            [Fact]
            public void AddCardToDiscardPile()
            {
                var discardCard = Card(1, Hearts);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(playedCard);
            }

            [Fact]
            public void RemoveCardFromPlayersHand()
            {
                var discardCard = Card(1, Hearts);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().NotContain(playedCard);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var discardCard = Card(1, Hearts);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

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
                var discardCard = Card(1, Hearts);
                var playedCard = Card(1, Clubs);
                var player2HandWithNoPlays = new CardCollection(
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds),
                    Card(6, Diamonds)
                );
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2HandWithNoPlays)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

                sut.Play(new PlayContext(sut.GetPlayer(1), playedCard));

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.NextAction.Should().Be(Action.Take);
            }

            [Fact]
            public void CreateNewTurnAfterPlayer2TurnWithValidPlay()
            {
                var discardCard = Card(1, Hearts);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                var playedCard2 = Card(1, Diamonds);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

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
                var discardCard = Card(10, Clubs);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                var playedCard2 = Card(1, Diamonds);
                var playedCard3 = Card(1, Spades);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

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
                var discardCard = Card(10, Clubs);
                var playedCard = Card(1, Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

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
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .BuildAtStartGame();
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
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .BuildAtStartGame();
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
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .BuildAtStartGame();

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
            private readonly Card originalDiscardCard;
            private readonly Card playedCard;
            private readonly Game sut;

            public PlayWhenInvalidShould()
            {
                originalDiscardCard = Card(10, Hearts);
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(7, Hearts)
                );
                var player2Hand = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(7, Diamonds),
                    Card(10, Diamonds)
                );
                var player3Hand = new CardCollection(
                    Card(1, Spades),
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(7, Spades),
                    Card(10, Spades)
                );
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                playedCard = Card(1, Clubs);

                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(originalDiscardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer1Hand()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), Card(1, Hearts)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer2Hand()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), Card(7, Hearts)));
                var actual = sut.Play(new PlayContext(sut.GetPlayer(2), Card(1, Hearts)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer3Hand()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), Card(7, Hearts)));
                sut.Play(new PlayContext(sut.GetPlayer(2), Card(7, Diamonds)));
                var actual = sut.Play(new PlayContext(sut.GetPlayer(3), Card(1, Hearts)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs1AndIsNotCurrentPlayer()
            {
                sut.Play(new PlayContext(sut.GetPlayer(1), Card(7, Hearts)));
                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), Card(7, Diamonds)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs2AndIsNotCurrentPlayer()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(2), Card(10, Diamonds)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenPlayerNumberIs3AndIsNotCurrentPlayer()
            {
                var actual = sut.Play(new PlayContext(sut.GetPlayer(3), Card(10, Spades)));

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

                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(Card(1, Spades))
                    .BuildAtStartGame();
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
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .BuildAtStartGame();

                sut.Play(new PlayContext(sut.GetPlayer(1), new Card(8, Clubs)));
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(1), Diamonds));

                var actual = sut.Play(new PlayContext(sut.GetPlayer(2), new Card(1, Diamonds)));
                actual.IsSuccess.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnTrue_ForPlayMatchingSelectedSuitAfterPlayer2()
            {
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(2)
                    .BuildAtStartGame();

                sut.Play(new PlayContext(sut.GetPlayer(2), new Card(8, Diamonds)));
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(2), Spades));

                var actual = sut.Play(new PlayContext(sut.GetPlayer(3), new Card(1, Spades)));
                actual.IsSuccess.Should().BeTrue();
            }
            
            [Fact]
            public void ReturnTrue_ForPlayMatchingSelectedSuitAfterPlayer3()
            {
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStartingPlayer(3)
                    .BuildAtStartGame();

                sut.Play(new PlayContext(sut.GetPlayer(3), new Card(8, Spades)));
                sut.SelectSuit(new SelectSuitContext(sut.GetPlayer(3), Clubs));

                var actual = sut.Play(new PlayContext(sut.GetPlayer(1), new Card(1, Clubs)));
                actual.IsSuccess.Should().BeTrue();
            }
        }

        public class TakeShould
        {
            private CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private CardCollection player3Hand;

            public TakeShould()
            {
                player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs),
                    Card(3, Clubs),
                    Card(4, Clubs),
                    Card(5, Clubs)
                );
                player2Hand = new CardCollection(
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds),
                    Card(10, Diamonds)
                );
                player3Hand = new CardCollection(
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades),
                    Card(6, Spades)
                );
            }

            [Fact]
            public void CreatesNewTurn()
            {
                var discardCard = Card(10, Hearts);
                var stockPile = new CardCollection(
                    Card(9, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();

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
                var discardCard = Card(10, Hearts);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();

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
                var discardCard = Card(10, Hearts);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();
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
                var discardCard = Card(10, Hearts);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();

                sut.Take(new TakeContext(sut.GetPlayer(1)));

                var actual = sut.GameState.CurrentTable;
                actual.Players[0].Hand.Cards.Should().Contain(new Card(1, Hearts));
                actual.StockPile.Cards.Should().BeEmpty();
            }

            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer2()
            {
                var discardCard = Card(1, Hearts);
                var stockPile = new CardCollection(
                    Card(1, Hearts),
                    Card(2, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();
                sut.Play(new PlayContext(sut.GetPlayer(1), player1Hand.Cards.First()));

                sut.Take(new TakeContext(sut.GetPlayer(2)));

                var actual = sut.GameState.CurrentTable;
                actual.Players[1].Hand.Cards.Should().Contain(new Card(1, Hearts));
                actual.StockPile.Cards.Should().Equal(Card(2, Hearts));
            }
            
            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer3()
            {
                var discardCard = Card(2, Hearts);
                player3Hand = new CardCollection(
                    Card(7, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades),
                    Card(6, Spades)
                );

                var stockPile = new CardCollection(
                    Card(3, Hearts),
                    Card(4, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();
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
                    Card(13, Spades),
                    Card(2, Hearts),
                    Card(3, Hearts),
                    Card(1, Hearts)
                ));
                discardPile.TurnUpTopCard();
                var stockPile = new StockPile(new CardCollection(
                    Card(1, Clubs)
                ));

                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                var currentTurn = new CurrentTurn(1, player1, Action.Take);
                
                var sut = new InProgressGameBuilder()
                    .WithCurrentPlayer(player1)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardPile(discardPile)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .Build();
                
                sut.Take(new TakeContext(sut.GetPlayer(1)));

                sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(Card(13, Spades));
                sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(
                    Card(2, Hearts),
                    Card(3, Hearts),
                    Card(1, Hearts)
                );

                var actualCard = sut.GameState.Events.ElementAt(1);
                actualCard.Should().BeOfType(typeof(CardMoved));
                var domainEvent = actualCard as CardMoved;
                if (domainEvent is null) Assert.NotNull(domainEvent);
                domainEvent.Card.Should().Be(Card(2, Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
                
                actualCard = sut.GameState.Events.ElementAt(2);
                actualCard.Should().BeOfType(typeof(CardMoved));
                domainEvent = actualCard as CardMoved;
                if (domainEvent is null) Assert.NotNull(domainEvent);
                domainEvent.Card.Should().Be(Card(3, Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
                
                actualCard = sut.GameState.Events.ElementAt(3);
                actualCard.Should().BeOfType(typeof(CardMoved));
                domainEvent = actualCard as CardMoved;
                if (domainEvent is null) Assert.NotNull(domainEvent);
                domainEvent.Card.Should().Be(Card(1, Hearts));
                domainEvent.FromSource.Should().Be(CardMoveSources.DiscardPile);
                domainEvent.ToSource.Should().Be(CardMoveSources.StockPile);
            }
            
            [Fact]
            public void ShuffleStockPileWhenMovingDiscardCardsToStockPile()
            {
                var expectedDiscardPileRestOfCards = new[]
                {
                    Card(5, Hearts),
                    Card(6, Hearts),
                    Card(4, Hearts)
                };
                var discardPile = new DiscardPile(new CardCollection(
                    expectedDiscardPileRestOfCards
                ));
                discardPile.AddCard(Card(7, Spades));
                var expectedShuffledStockPile = new[]
                {
                    Card(12, Spades),
                    Card(13, Clubs),
                    Card(11, Diamonds)
                };

                var shufflerMock = new Mock<IShuffler>();
                shufflerMock.Setup(x => x.Shuffle(It.Is<CardCollection>(y => y.Cards.ToArray().SequenceEqual(expectedDiscardPileRestOfCards))))
                    .Returns(new CardCollection(expectedShuffledStockPile
                ));
                var stockPile = new StockPile(new CardCollection(
                    Card(1, Clubs)
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
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
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
                var discardCard = Card(13, Spades);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(2)
                    .BuildAtStartGame();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(1)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }
            
            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer2()
            {
                var discardCard = Card(13, Spades);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(1)
                    .BuildAtStartGame();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(2)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void ReturnNotPlayersTurnWhenPlayer3()
            {
                var discardCard = Card(13, Hearts);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(1)
                    .BuildAtStartGame();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(3)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }
            
            [Fact]
            public void ReturnInvalidTakeWhenNextActionIsNotTake()
            {
                var discardCard = Card(10, Clubs);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(1)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void ReturnInvalidTakeWhenFirstTurnAndCardToMatchIsEight()
            {
                var discardCard = Card(8, Clubs);
                var stockPile = new CardCollection(
                    Card(1, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();

                var actual = sut.Take(new TakeContext(sut.GetPlayer(1)));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
                sut.GameState.PreviousTurnResult.Should().BeNull();
            }

            [Fact]
            public void CreateTakenEvent()
            {
                var discardCard = Card(10, Hearts);
                var stockPile = new CardCollection(
                    Card(9, Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .BuildAtStartGame();

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
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private readonly CardCollection player3Hand;

            public PlayWhenWonShould()
            {
                player1Hand = new CardCollection(Card(1, Clubs));
                player2Hand = new CardCollection(Card(2, Diamonds));
                player3Hand = new CardCollection(Card(3, Spades));

                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player2 = sampleData.Player2;
                player3 = sampleData.Player3;

                stockPile = new StockPile(new CardCollection());
            }

            [Fact]
            public void ReturnTrue_WhenPlayer1WonAfterPlay()
            {
                var discardCard = Card(1, Hearts);
                var currentTurn = new CurrentTurn(7, player1, Action.Play);
                var sut = new InProgressGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player1)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), player1Hand.Cards.First()));

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
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player2)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(2), player2Hand.Cards.First()));

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
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player3)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(3), player3Hand.Cards.First()));

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
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithPlayer3Hand(player3Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .WithCurrentPlayer(player1)
                    .Build();

                sut.Play(new PlayContext(sut.GetPlayer(1), player1Hand.Cards.First()));

                sut.GameState.Events.Last().Should().BeOfType(typeof(RoundWon));
                var domainEvent = sut.GameState.Events.Last() as RoundWon;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
            }
        }
    }
}
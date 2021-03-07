using System.Linq;
using FluentAssertions;
using Moq;
using SheddingCardGames.Domain;
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
                var rules = new Rules(7);
                var sut = new Game(rules, new DummyShuffler(), new Dealer(rules ), new[] {new Player(1, "Alice"), new Player(2, "Bob")}, new CardCollectionBuilder().Build());

                sut.GameState.CurrentTurn.Should().BeNull();
                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.New);
            }
        }

        public class InitialiseShould
        {
            private Game sut;

            public InitialiseShould()
            {
                var rules = new Rules(7);
                sut = new Game(rules, new DummyShuffler(), new Dealer(rules), new[] { new Player(1, "Alice"), new Player(2, "Bob") }, new CardCollectionBuilder().Build());
            }
            
            [Fact]
            public void SetGameStateWhenGamePhaseIsNew()
            {
                var gameState = new GameState(GamePhase.New);
                
                sut.Initialise(gameState);

                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.New);
                sut.GameState.CurrentBoard.Should().BeNull();
                sut.GameState.StartingPlayer.Should().BeNull();
                sut.GameState.CurrentTurn.Should().BeNull();
            }
            
            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public void SetGameStateWhenGamePhaseIsReadyToDeal(int expectedStartingPlayer)
            {
                var gameState = new GameState(GamePhase.ReadyToDeal, expectedStartingPlayer);
                
                sut.Initialise(gameState);

                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.ReadyToDeal);
                sut.GameState.CurrentBoard.Should().BeNull();
                sut.GameState.StartingPlayer.Should().Be(expectedStartingPlayer);
                sut.GameState.CurrentTurn.Should().BeNull();
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public void SetGameStateWhenGamePhaseIsInGame(int expectedStartingPlayerNumber)
            {
                var player1Hand = new CardCollection(new Card(1, Suit.Clubs));
                var player2Hand = new CardCollection(new Card(1, Suit.Diamonds));
                var stockPile = new StockPile(new CardCollection(new Card(1, Suit.Hearts), new Card(2, Suit.Hearts)));
                var discardPile = new DiscardPile(new[]
                {
                    new Card(1, Suit.Spades),
                    new Card(2, Suit.Spades)
                });
                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var expectedStartingPlayer = sampleData.GetPlayer(expectedStartingPlayerNumber);
                var expectedTurn = new Turn(1, expectedStartingPlayer, new Card[0],
                    false, null, Action.Play, null);
                sut = new InProgressGameBuilder()
                    .WithStartingPlayer(expectedStartingPlayer.Number)
                    .WithPlayer1(player1)
                    .WithPlayer2(player2)
                    .WithStockPile(stockPile)
                    .WithDiscardPile(discardPile)
                    .WithCurrentTurn(expectedTurn)
                    .Build();
                
                sut.GameState.CurrentGamePhase.Should().Be(GamePhase.InGame);
                sut.GameState.StartingPlayer.Should().Be(expectedStartingPlayer.Number);
                sut.GameState.CurrentBoard.Player1.Hand.Should().Be(player1Hand);
                sut.GameState.CurrentBoard.Player2.Hand.Should().Be(player2Hand);
                sut.GameState.CurrentBoard.StockPile.Should().Be(stockPile);
                sut.GameState.CurrentBoard.DiscardPile.Should().Be(discardPile);
                sut.GameState.CurrentTurn.Should().Be(expectedTurn);
            }
        }

        public class ChooseStartingPlayerShould
        {
            private readonly Game sut;

            public ChooseStartingPlayerShould()
            {
                var shuffler = new DummyShuffler();
                var rules = new Rules(7);
                var deck = new DeckBuilder().Build();
                sut = new Game(rules, shuffler, new Dealer(rules), new[] { new Player(1, "Alice"), new Player(2, "Bob") }, deck);
            }

            [Theory]
            [InlineData(1)]
            [InlineData(2)]
            public void SetStartingPlayerToPlayer1(int startingPlayer)
            {
                sut.ChooseStartingPlayer(startingPlayer);
                
                sut.GameState.StartingPlayer.Should().Be(startingPlayer);
            }

            [Fact]
            public void SetGamePhaseToReadyToDeal1()
            {
                sut.ChooseStartingPlayer(1);
                
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
                var stockPile = new CardCollection(new Card(1, Suit.Spades));
                var discardCard = new Card(2, Suit.Hearts);

                sut = new ReadyToDealGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
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
                stockPile = new CardCollection(new Card(1, Suit.Spades));
            }

            private Game CreateSut(int withStartingPlayer, Card withDiscardCard, IShuffler withShuffler = null)
            {
                if (withShuffler == null)
                    withShuffler = new DummyShuffler();
                
                return new ReadyToDealGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithStockPile(stockPile)
                    .WithDiscardCard(withDiscardCard)
                    .WithStartingPlayer(withStartingPlayer)
                    .WithShuffler(withShuffler)
                    .Build();
            }

            [Fact]
            public void ShuffleCards()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var expectedDeck = new SpecificDeckBuilder(player1Hand, player2Hand, discardCard, stockPile).Build();
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
            public void PreserveStartingPlayer(int expectedStartingPlayer)
            {
                var discardCard = new Card(2, Suit.Hearts);
                var sut = CreateSut(expectedStartingPlayer, discardCard);

                sut.Deal();

                sut.GameState.StartingPlayer.Should().Be(expectedStartingPlayer);
            }
            
            [Fact]
            public void AddHandToPlayer1()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().Equal(player1Hand.Cards);
            }

            [Fact]
            public void AddHandToPlayer2()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                var actual = sut.GameState.CurrentBoard;
                actual.Player2.Hand.Cards.Should().Equal(player2Hand.Cards);
            }

            [Fact]
            public void SetupDiscardPile()
            {
                var expectedDiscardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, expectedDiscardCard);

                sut.Deal();

                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(expectedDiscardCard);
            }

            [Fact]
            public void SetupStockPile()
            {
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

                sut.GameState.CurrentBoard.StockPile.Cards.Should().Equal(stockPile.Cards);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer1()
            {
                var expectedValidPlaysForPlayer1 = new CardCollection(new Card(2, Suit.Clubs));
                var discardCard = new Card(2, Suit.Hearts);
                var startingPlayer = 1;
                var sut = CreateSut(startingPlayer, discardCard);

                sut.Deal();

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

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(1);
                actual.HasWinner.Should().BeFalse();
                actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer2.Cards);
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
            private Game sut;

            public PlayWhenValidShould()
            {
                deck = new CardCollectionBuilder().Build();

                player1Hand = new CardCollection(
                    deck.Get(2, Suit.Clubs),
                    deck.Get(1, Suit.Hearts)
                );
                player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds)
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
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);

                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(playedCard);
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
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);

                sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().NotContain(playedCard);
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

                sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().NotContain(playedCard);
                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(playedCard);
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
            public void CreateNewTurnForAnotherValidPlay()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                var playedCard2 = deck.Get(1, Suit.Diamonds);
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, playedCard);
                sut.Play(2, playedCard2);

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(3);
                actual.PlayerToPlay.Number.Should().Be(1);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(1, Suit.Hearts));
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.NextAction.Should().Be(Action.Play);

                sut.GameState.CurrentBoard.Player2.Hand.Cards.Should().NotContain(playedCard2);
                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(playedCard2);
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
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
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

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(1);
                actual.PlayerToPlay.Number.Should().Be(1);
                actual.NextAction.Should().Be(Action.SelectSuit);
                
                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(cardToPlay);
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
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(deck.Get(1, Suit.Spades))
                    .Build();

                sut.Play(1, deck.Get(1, Suit.Clubs));

                sut.Play(2, deck.Get(8, Suit.Diamonds));
                
                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.NextAction.Should().Be(Action.SelectSuit);
                
                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(deck.Get(8, Suit.Diamonds));
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
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                playedCard = deck.Get(1, Suit.Clubs);

                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
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

            [Theory]
            [InlineData(0)]
            [InlineData(3)]
            public void ReturnFalseWhenPlayerNumberIsInvalid(int playerNumber)
            {
                var actual = sut.Play(playerNumber, deck.Get(7, Suit.Hearts));

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
            public void ReturnFalseWhenPlayerNumberIs1AndIsNotCurrentPlayer()
            {
                sut.Play(1, deck.Get(7, Suit.Hearts));
                var actual = sut.Play(1, deck.Get(7, Suit.Diamonds));

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnFalseWhenItIsNotCurrentPlayersTurn()
            {
                var actual = sut.Play(2, deck.Get(10, Suit.Diamonds));

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

                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(originalDiscardCard);
            }

            [Fact]
            public void NotRemoveCardFromPlayersHand()
            {
                sut.Play(1, playedCard);

                sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().Contain(playedCard);
            }

            [Fact]
            public void NotCreateNewTurn()
            {
                sut.Play(1, playedCard);

                var actual = sut.GameState.CurrentTurn;
                
                actual.TurnNumber.Should().Be(1);
            }
        }

        public class PlayWhenWonShould
        {
            private readonly Card discardCard;
            private CardCollection player1Hand;
            private CardCollection player2Hand;
            private Game sut;

            public PlayWhenWonShould()
            {
                discardCard = new Card(13, Suit.Diamonds);
                var rules = new Rules(7);
                sut = new Game(rules, new DummyShuffler(), new Dealer(rules), new[] {new Player(1, "Alice"), new Player(2, "Bob")}, new CardCollectionBuilder().Build());
            }

            [Fact]
            public void ReturnTrueWhenPlayer1WonAtSetup()
            {
                player1Hand = new CardCollection();
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

                var actual = sut.GameState.CurrentTurn;
                
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(1);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
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

                var actual = sut.GameState.CurrentTurn;
                
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(1);
                actual.TurnNumber.Should().Be(1);
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

                var actual = sut.GameState.CurrentTurn;
                
                actual.HasWinner.Should().BeTrue();
                actual.Winner.Number.Should().Be(2);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }
        }

        public class PlayWhenSelectedSuitShould
        {
            private readonly Card discardCard;
            private CardCollection player1Hand;
            private CardCollection player2Hand;
            private Game sut;

            public PlayWhenSelectedSuitShould()
            {
                discardCard = new Card(11, Suit.Diamonds);
            }

            [Fact]
            public void ReturnTrueForPlayMatchingSelectedSuit()
            {
                player1Hand = new CardCollection
                (
                    new Card(8, Suit.Clubs),
                    new Card(2, Suit.Clubs)
                );
                player2Hand = new CardCollection
                (
                    new Card(4, Suit.Clubs),
                    new Card(1, Suit.Hearts)
                );
                sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(1, new Card(8, Suit.Clubs));
                sut.SelectSuit(1, Suit.Hearts);

                var actual = sut.Play(2, new Card(1, Suit.Hearts));
                actual.IsSuccess.Should().BeTrue();
            }
        }

        public class SelectSuit
        {
            private readonly Game sut;

            public SelectSuit()
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

                sut.Take(0);

                sut.GameState.CurrentTurn.SelectedSuit.Should().Be(Suit.Diamonds);
            }
            
            [Fact]
            public void ReturnTrueWhenTakeIsValid()
            {
                var actual = sut.SelectSuit(1, Suit.Diamonds);

                actual.IsSuccess.Should().BeTrue();
                actual.MessageKey.Should().Be(ActionResultMessageKey.Success);
            }
            
            [Theory]
            [InlineData(0)]
            [InlineData(3)]
            public void ReturnFalseWhenPlayerNumberInvalid(int playerNumber)
            {
                var actual = sut.SelectSuit(playerNumber, Suit.Diamonds);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }
            
            [Fact]
            public void ReturnFalseWhenNotPlayersTurn()
            {
                var actual = sut.SelectSuit(2, Suit.Diamonds);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
            }
        }

        public class TakeShould
        {
            private readonly CardCollection deck;
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;

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
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(1);

                var actual = sut.GameState.CurrentTurn;
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Number.Should().Be(2);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(10, Suit.Diamonds));
                actual.NextAction.Should().Be(Action.Play);
                
                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
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
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(1);

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
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take(1);

                var actual = sut.GameState.CurrentBoard;
                actual.Player1.Hand.Cards.Should().Contain(new Card(1, Suit.Hearts));
                sut.GameState.CurrentBoard.StockPile.Cards.Should().BeEmpty();
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
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(1, player1Hand.Cards.First());

                sut.Take(2);

                var actual = sut.GameState.CurrentBoard;
                actual.Player2.Hand.Cards.Should().Contain(new Card(1, Suit.Hearts));
                actual.StockPile.Cards.Should().Equal(deck.Get(2, Suit.Hearts));
            }

            [Fact]
            public void MoveDiscardCardsToStockPileWhenStockPileExhausted()
            {
                var discardPile = new DiscardPile( new CardCollection(
                    deck.Get(1, Suit.Spades),
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

                var currentTurn = new Turn(1, player1, new Card[0], false, null, Action.Take, null);
                
                var sut = new InProgressGameBuilder()
                    .WithStartingPlayer(1)
                    .WithPlayer1(player1)
                    .WithPlayer2(player2)
                    .WithDiscardPile(discardPile)
                    .WithStockPile(stockPile)
                    .WithCurrentTurn(currentTurn)
                    .Build();
                
                sut.Take(1);

                sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(deck.Get(1, Suit.Spades));
                sut.GameState.CurrentBoard.StockPile.Cards.Should().Equal(
                    deck.Get(2, Suit.Hearts),
                    deck.Get(3, Suit.Hearts),
                    deck.Get(1, Suit.Hearts)
                );
                sut.CardMoves.Should().Equal(
                    new CardMoveEvent(deck.Get(1, Suit.Clubs), CardMoveSources.StockPile, CardMoveSources.Player1Hand),
                    new CardMoveEvent(deck.Get(2, Suit.Hearts), CardMoveSources.DiscardPile, CardMoveSources.StockPile),
                    new CardMoveEvent(deck.Get(3, Suit.Hearts), CardMoveSources.DiscardPile, CardMoveSources.StockPile),
                    new CardMoveEvent(deck.Get(1, Suit.Hearts), CardMoveSources.DiscardPile, CardMoveSources.StockPile)
                );
            }

            [Theory]
            [InlineData(0)]
            [InlineData(3)]
            public void ReturnFalseWhenInvalidPlayerNumber(int playerNumber)
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(playerNumber);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                actual.Card.Should().Be(null);
            }

            [Fact]
            public void ReturnFalseWhenNotPlayersTurnWhenPlayer1()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(2);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.NotPlayersTurn);
                actual.Card.Should().Be(null);
            }
            
            [Fact]
            public void ReturnFalseWhenNotPlayersTurnWhenPlayer2()
            {
                var discardCard = deck.Get(1, Suit.Spades);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
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
            public void ReturnFalseWhenNextActionIsNotTake()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new AtStartGameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take(1);

                actual.IsSuccess.Should().BeFalse();
                actual.MessageKey.Should().Be(ActionResultMessageKey.InvalidTake);
                actual.Card.Should().Be(null);
            }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Xunit;

namespace SheddingCardGames.Tests
{
    namespace GameTests
    {
        public class ConstructorShould
        {
            [Fact]
            public void CreateGameAtNewState()
            {
                var sut = new Game(new Rules(), new DummyShuffler(), new[] {new Player(1), new Player(2)});

                sut.GetCurrentTurn().Should().BeNull();
            }
        }

        public class SetupShould
        {
            private readonly Card expectedDiscardCard;
            private readonly CardCollection expectedValidPlaysForPlayer1;
            private readonly CardCollection expectedValidPlaysForPlayer2;
            private readonly CardCollection player1Hand;
            private readonly CardCollection player2Hand;
            private readonly CardCollection stockPile;
            private Game sut;

            public SetupShould()
            {
                player1Hand = new CardCollection(
                    new Card(1, Suit.Clubs),
                    new Card(2, Suit.Clubs)
                );
                player2Hand = new CardCollection(
                    new Card(1, Suit.Diamonds),
                    new Card(2, Suit.Diamonds)
                );
                expectedDiscardCard = new Card(2, Suit.Hearts);

                stockPile = new CardCollection(new Card(1, Suit.Spades));
                expectedValidPlaysForPlayer1 = new CardCollection(new Card(2, Suit.Clubs));
                expectedValidPlaysForPlayer2 = new CardCollection(new Card(2, Suit.Diamonds));

                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(expectedDiscardCard)
                    .WithStockPile(stockPile)
                    .Build();
            }

            [Fact]
            public void AddHandToPlayer1()
            {
                var actual = sut.GetCurrentTurn();
                actual.Player1Hand.Cards.Should().Equal(player1Hand.Cards);
            }

            [Fact]
            public void AddHandToPlayer2()
            {
                var actual = sut.GetCurrentTurn();
                actual.Player2Hand.Cards.Should().Equal(player2Hand.Cards);
            }

            [Fact]
            public void SetupDiscardPile()
            {
                var actual = sut.GetCurrentTurn();
                actual.DiscardPile.CardToMatch.Should().Be(expectedDiscardCard);
            }

            [Fact]
            public void SetupStockPile()
            {
                var actual = sut.GetCurrentTurn();
                actual.StockPile.Should().Equal(stockPile.Cards);
            }

            [Fact]
            public void SetStartingPlayerToPlayer1()
            {
                var actual = sut.GetCurrentTurn();
                actual.PlayerToPlay.Should().Be(1);
            }

            [Fact]
            public void SetStartingPlayerToPlayer2()
            {
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(expectedDiscardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(2)
                    .Build();

                var actual = sut.GetCurrentTurn();
                actual.PlayerToPlay.Should().Be(2);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer1()
            {
                var actual = sut.GetCurrentTurn();
                actual.TurnNumber.Should().Be(1);
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer1.Cards);
                actual.NextAction.Should().Be(Action.Play);
            }

            [Fact]
            public void CreateFirstTurnWithNextActionTake()
            {
                var discardCard = new Card(10, Suit.Spades);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(2)
                    .Build();

                var actual = sut.GetCurrentTurn();
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Take);
            }

            [Fact]
            public void CreateFirstTurnWhenStartingWithPlayer2()
            {
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(expectedDiscardCard)
                    .WithStockPile(stockPile)
                    .WithStartingPlayer(2)
                    .Build();

                var actual = sut.GetCurrentTurn();
                actual.TurnNumber.Should().Be(1);
                actual.HasWinner.Should().BeFalse();
                actual.ValidPlays.Should().Equal(expectedValidPlaysForPlayer2.Cards);
                actual.NextAction.Should().Be(Action.Play);
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
                    deck.Get(3, Suit.Clubs),
                    deck.Get(4, Suit.Clubs),
                    deck.Get(5, Suit.Clubs),
                    deck.Get(6, Suit.Clubs),
                    deck.Get(1, Suit.Hearts)
                );
                player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds),
                    deck.Get(4, Suit.Diamonds),
                    deck.Get(5, Suit.Diamonds),
                    deck.Get(6, Suit.Diamonds),
                    deck.Get(7, Suit.Diamonds)
                );
            }


            [Fact]
            public void ReturnTrueWhenMatchingSuit()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(playedCard);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrueWhenMatchingRank()
            {
                var discardCard = deck.Get(10, Suit.Clubs);
                var playedCard = deck.Get(10, Suit.Hearts);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(playedCard);

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrueWhenRank8()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var playedCard = deck.Get(8, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                var actual = sut.Play(playedCard);

                actual.Should().BeTrue();
            }

            [Fact]
            public void AddCardToDiscardPile()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(playedCard);

                sut.GetCurrentTurn().DiscardPile.CardToMatch.Should().Be(playedCard);
            }

            [Fact]
            public void RemoveCardFromPlayersHand()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(playedCard);

                var actual = sut.GetCurrentTurn();
                actual.Player1Hand.Cards.Should().NotContain(playedCard);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(playedCard);

                var actual = sut.GetCurrentTurn();

                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Should().Be(2);
                actual.Player1Hand.Cards.Should().NotContain(playedCard);
                actual.DiscardPile.CardToMatch.Should().Be(playedCard);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(1, Suit.Diamonds));
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.NextAction.Should().Be(Action.Play);
            }

            [Fact]
            public void CreateNewTurnWithNextActionTake()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var playedCard = deck.Get(1, Suit.Clubs);
                var player2Hand = new CardCollection(
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds),
                    deck.Get(4, Suit.Diamonds),
                    deck.Get(5, Suit.Diamonds),
                    deck.Get(6, Suit.Diamonds),
                    deck.Get(7, Suit.Diamonds),
                    deck.Get(9, Suit.Diamonds)
                );

                player1Hand.AddAtStart(playedCard);
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(playedCard);

                var actual = sut.GetCurrentTurn();

                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Should().Be(2);
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
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .Build();

                sut.Play(playedCard);
                sut.Play(playedCard2);

                var actual = sut.GetCurrentTurn();

                actual.TurnNumber.Should().Be(3);
                actual.PlayerToPlay.Should().Be(1);
                actual.Player2Hand.Cards.Should().NotContain(playedCard2);
                actual.DiscardPile.CardToMatch.Should().Be(playedCard2);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(1, Suit.Hearts));
                actual.HasWinner.Should().BeFalse();
                actual.Winner.Should().BeNull();
                actual.NextAction.Should().Be(Action.Play);
            }
        }

        public class PlayWhenPlayingEightShould
        {
            private readonly CardCollection deck;
            private readonly Game sut;

            public PlayWhenPlayingEightShould()
            {
                deck = new CardCollectionBuilder().Build();

                var player1Hand = new CardCollection(
                    deck.Get(1, Suit.Clubs),
                    deck.Get(8, Suit.Clubs)
                );
                var player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds),
                    deck.Get(4, Suit.Diamonds),
                    deck.Get(5, Suit.Diamonds),
                    deck.Get(6, Suit.Diamonds),
                    deck.Get(7, Suit.Diamonds)
                );
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(deck.Get(1, Suit.Spades))
                    .Build();
            }

            [Fact]
            public void ReturnTrue()
            {
                var actual = sut.Play(deck.Get(8, Suit.Clubs));

                actual.Should().BeTrue();
            }

            [Fact]
            public void CreateCrazyEightTurn()
            {
                var cardToPlay = deck.Get(8, Suit.Clubs);

                sut.Play(cardToPlay);

                var actual = sut.GetCurrentTurn();

                actual.TurnNumber.Should().Be(1);
                actual.PlayerToPlay.Should().Be(1);
                actual.NextAction.Should().Be(Action.SelectSuit);
                actual.DiscardPile.CardToMatch.Should().Be(cardToPlay);
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
                    deck.Get(10, Suit.Diamonds)
                );

                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(deck.Get(1, Suit.Spades))
                    .Build();
                sut.Play(deck.Get(8, Suit.Clubs));
            }

            [Fact]
            public void AddSelectedSuitToTurn()
            {
                sut.SelectSuit(Suit.Diamonds);

                var actualPreviousTurn = sut.Turns.ElementAt(0);
                actualPreviousTurn.TurnNumber.Should().Be(1);
                actualPreviousTurn.NextAction.Should().Be(Action.SelectSuit);

                var actualCurrentTurn = sut.GetCurrentTurn();
                actualCurrentTurn.TurnNumber.Should().Be(2);
                actualCurrentTurn.PlayerToPlay.Should().Be(2);
                actualCurrentTurn.ValidPlays.Should().Equal(new Card(10, Suit.Diamonds));
                actualCurrentTurn.NextAction.Should().Be(Action.Play);
                actualCurrentTurn.SelectedSuit.Should().Be(Suit.Diamonds);
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
                    deck.Get(3, Suit.Clubs),
                    deck.Get(4, Suit.Clubs),
                    deck.Get(5, Suit.Clubs),
                    deck.Get(6, Suit.Clubs),
                    deck.Get(7, Suit.Hearts)
                );
                var player2Hand = new CardCollection(
                    deck.Get(1, Suit.Diamonds),
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds),
                    deck.Get(4, Suit.Diamonds),
                    deck.Get(5, Suit.Diamonds),
                    deck.Get(6, Suit.Diamonds),
                    deck.Get(10, Suit.Diamonds)
                );
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                playedCard = deck.Get(1, Suit.Clubs);

                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(originalDiscardCard)
                    .WithStockPile(stockPile)
                    .Build();
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer1Hand()
            {
                var actual = sut.Play(deck.Get(1, Suit.Hearts));

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnFalseWhenCardIsNotInPlayer2Hand()
            {
                sut.Play(deck.Get(7, Suit.Hearts));
                var actual = sut.Play(deck.Get(1, Suit.Hearts));

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnFalse()
            {
                var actual = sut.Play(playedCard);

                actual.Should().BeFalse();
            }

            [Fact]
            public void NotAddCardToDiscardPile()
            {
                sut.Play(playedCard);

                var actual = sut.GetCurrentTurn();
                actual.DiscardPile.CardToMatch.Should().Be(originalDiscardCard);
            }

            [Fact]
            public void NotRemoveCardFromPlayersHand()
            {
                sut.Play(playedCard);

                var actual = sut.GetCurrentTurn();
                actual.Player1Hand.Cards.Should().Contain(playedCard);
            }

            [Fact]
            public void NotCreateNewTurn()
            {
                sut.Play(playedCard);

                var actual = sut.GetCurrentTurn();
                actual.TurnNumber.Should().Be(1);
            }
        }

        public class PlayWhenWonShould
        {
            private readonly CardCollection discardPile;
            private CardCollection player1Hand;
            private CardCollection player2Hand;
            private Game sut;

            public PlayWhenWonShould()
            {
                discardPile = new CardCollection(
                    new Card(13, Suit.Diamonds),
                    new Card(11, Suit.Diamonds)
                );
                sut = new Game(new Rules(), new DummyShuffler(), new[] {new Player(1), new Player(2)});
            }

            [Fact]
            public void ReturnTrueWhenPlayer1WonAtSetup()
            {
                player1Hand = new CardCollection();

                player2Hand = new CardCollection(new Card(2, Suit.Clubs), new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs), new Card(8, Suit.Clubs), new Card(10, Suit.Clubs),
                    new Card(12, Suit.Clubs), new Card(1, Suit.Diamonds));
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build();

                var actual = sut.GetCurrentTurn();

                actual.HasWinner.Should().BeTrue();
                actual.Winner.Should().Be(1);
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
                    new Card(6, Suit.Clubs),
                    new Card(8, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(12, Suit.Clubs),
                    new Card(1, Suit.Diamonds));
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build();
                sut.Play(player1Hand.Cards.First());

                var actual = sut.GetCurrentTurn();

                actual.HasWinner.Should().BeTrue();
                actual.Winner.Should().Be(1);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }

            [Fact]
            public void ReturnTrueWhenPlayer2WonAtSetup()
            {
                player1Hand = new CardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(8, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(12, Suit.Clubs),
                    new Card(1, Suit.Diamonds));
                player2Hand = new CardCollection();
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .WithStartingPlayer(2)
                    .Build();

                var actual = sut.GetCurrentTurn();

                actual.HasWinner.Should().BeTrue();
                actual.Winner.Should().Be(2);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }

            [Fact]
            public void ReturnTrueWhenPlayer2WonAfterPlay()
            {
                player1Hand = new CardCollection(
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(8, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(12, Suit.Clubs),
                    new Card(1, Suit.Diamonds)
                );
                player2Hand = new CardCollection(new Card(13, Suit.Clubs));
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .WithStartingPlayer(2)
                    .Build();
                sut.Play(player2Hand.Cards.First());

                var actual = sut.GetCurrentTurn();

                actual.HasWinner.Should().BeTrue();
                actual.Winner.Should().Be(2);
                actual.TurnNumber.Should().Be(1);
                actual.ValidPlays.Should().BeEmpty();
                actual.NextAction.Should().Be(Action.Won);
            }
        }

        public class PlayWhenSelectedSuitShould
        {
            private readonly CardCollection discardPile;
            private CardCollection player1Hand;
            private CardCollection player2Hand;
            private Game sut;

            public PlayWhenSelectedSuitShould()
            {
                discardPile = new CardCollection(
                    new Card(11, Suit.Diamonds),
                    new Card(13, Suit.Diamonds)
                );
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
                    new Card(2, Suit.Clubs),
                    new Card(4, Suit.Clubs),
                    new Card(6, Suit.Clubs),
                    new Card(10, Suit.Clubs),
                    new Card(12, Suit.Clubs),
                    new Card(1, Suit.Hearts)
                );
                sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build();

                sut.Play(new Card(8, Suit.Clubs));
                sut.SelectSuit(Suit.Hearts);

                var actual = sut.Play(new Card(1, Suit.Hearts));
                actual.Should().BeTrue();
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
                    deck.Get(3, Suit.Clubs),
                    deck.Get(4, Suit.Clubs),
                    deck.Get(5, Suit.Clubs),
                    deck.Get(6, Suit.Clubs),
                    deck.Get(7, Suit.Clubs)
                );
                player2Hand = new CardCollection(
                    deck.Get(2, Suit.Diamonds),
                    deck.Get(3, Suit.Diamonds),
                    deck.Get(4, Suit.Diamonds),
                    deck.Get(5, Suit.Diamonds),
                    deck.Get(6, Suit.Diamonds),
                    deck.Get(7, Suit.Diamonds),
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
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take();

                var actual = sut.GetCurrentTurn();
                actual.TurnNumber.Should().Be(2);
                actual.PlayerToPlay.Should().Be(2);
                actual.DiscardPile.CardToMatch.Should().Be(discardCard);
                actual.ValidPlays.Should().BeEquivalentTo(new Card(10, Suit.Diamonds));
                actual.NextAction.Should().Be(Action.Play);
            }

            [Fact]
            public void ReturnCardTaken()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                var actual = sut.Take();

                actual.Should().Be(new Card(1, Suit.Hearts));
            }

            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer1()
            {
                var discardCard = deck.Get(10, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();

                sut.Take();

                var actual = sut.GetCurrentTurn();
                actual.Player1Hand.Cards.Should().Contain(new Card(1, Suit.Hearts));
                actual.StockPile.Should().BeEmpty();
            }

            [Fact]
            public void MoveCardFromStockPileToPlayersHandWhenPlayer2()
            {
                var discardCard = deck.Get(1, Suit.Hearts);
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Hearts),
                    deck.Get(2, Suit.Hearts)
                );
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardCard(discardCard)
                    .WithStockPile(stockPile)
                    .Build();
                sut.Play(player1Hand.Cards.First());

                sut.Take();

                var actual = sut.GetCurrentTurn();
                actual.Player2Hand.Cards.Should().Contain(new Card(1, Suit.Hearts));
                actual.StockPile.Should().Equal(deck.Get(2, Suit.Hearts));
            }

            [Fact]
            public void MoveDiscardCardsToStockPileWhenStockPileExhausted()
            {
                var player1Hand = new CardCollection(
                    deck.Get(7, Suit.Diamonds),
                    deck.Get(8, Suit.Diamonds),
                    deck.Get(9, Suit.Diamonds),
                    deck.Get(10, Suit.Diamonds),
                    deck.Get(11, Suit.Diamonds),
                    deck.Get(12, Suit.Diamonds),
                    deck.Get(13, Suit.Diamonds)
                );
                var player2Hand = new CardCollection(
                    deck.Get(7, Suit.Diamonds),
                    deck.Get(8, Suit.Diamonds),
                    deck.Get(9, Suit.Diamonds),
                    deck.Get(10, Suit.Diamonds),
                    deck.Get(11, Suit.Diamonds),
                    deck.Get(12, Suit.Diamonds),
                    deck.Get(13, Suit.Diamonds)
                );
                var discardPile = new CardCollection(
                    deck.Get(1, Suit.Spades),
                    deck.Get(1, Suit.Hearts),
                    deck.Get(2, Suit.Hearts),
                    deck.Get(3, Suit.Hearts)
                );
                var stockPile = new CardCollection(
                    deck.Get(1, Suit.Clubs)
                );
                var shuffledDiscardPile = new[]
                {
                    deck.Get(2, Suit.Hearts),
                    deck.Get(3, Suit.Hearts),
                    deck.Get(1, Suit.Hearts)
                };
                var shufflerMock = new Mock<IShuffler>();
                shufflerMock.Setup(x => x.Shuffle(It.IsAny<IEnumerable<Card>>())).Returns(shuffledDiscardPile);
                var sut = new GameBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .WithStockPile(stockPile)
                    .WithShuffler(shufflerMock.Object)
                    .Build();

                sut.Take();

                var actual = sut.GetCurrentTurn();

                actual.DiscardPile.CardToMatch.Should().Be(deck.Get(1, Suit.Spades));
                actual.StockPile.Should().Equal(
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
        }
    }
}
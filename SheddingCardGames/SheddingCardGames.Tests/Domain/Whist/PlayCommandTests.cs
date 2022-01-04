using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.Domain.Whist;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;
using GameState = SheddingCardGames.Domain.Whist.GameState;
using PlayCommand = SheddingCardGames.Domain.Whist.PlayCommand;
using PlayContext = SheddingCardGames.Domain.Whist.PlayContext;

namespace SheddingCardGames.Tests.Domain.Whist
{
    namespace PlayCommandTests
    {
        public class PlayCommandBuilder
        {
            private readonly List<Player> players = new List<Player>();
            private Player playerToPlay;
            private Player executingPlayer;
            private ImmutableList<Play> previousPlays;

            public PlayCommandBuilder WithPlayer1(Player player1, CardCollection withPlayer1Hand)
            {
                player1.Hand = withPlayer1Hand;
                players.Add(player1);
                return this;
            }

            public PlayCommandBuilder WithPlayer2(Player player2, CardCollection withPlayer2Hand)
            {
                player2.Hand = withPlayer2Hand;
                players.Add(player2);
                return this;
            }

            public PlayCommandBuilder WithPlayer3(Player player3, CardCollection withPlayer3Hand)
            {
                player3.Hand = withPlayer3Hand;
                players.Add(player3);
                return this;
            }

            public PlayCommandBuilder WithPlayer4(Player player4, CardCollection withPlayer4Hand)
            {
                player4.Hand = withPlayer4Hand;
                players.Add(player4);
                return this;
            }

            public PlayCommandBuilder WithPlayerToPlay(Player withPlayerToPlay)
            {
                playerToPlay = withPlayerToPlay;
                return this;
            }
            
            public PlayCommandBuilder WithExecutingPlayer(Player withExecutingPlayer)
            {
                executingPlayer = withExecutingPlayer;
                return this;
            }
            
            public PlayCommandBuilder WithPreviousPlays(ImmutableList<Play> withPreviousPlays)
            {
                previousPlays = withPreviousPlays;
                return this;
            }
            
            public PlayCommand Build(Card cardPlayed)
            {
                playerToPlay ??= players.First();
                executingPlayer ??= players.First();

                Trick trick = null;

                if (previousPlays != null)
                    trick = new Trick(new CardCollection(previousPlays.Select(x => x.Card)));

                var rules = new Rules();
                var gameState = new GameState
                {
                    CurrentTable = new Table(new StockPile(new CardCollection()), Players(players), trick),
                    CurrentStateOfPlay = new StateOfPlay(),
                    CurrentStateOfTrick = new StateOfTrick(1, players.First(), playerToPlay, previousPlays)
                };

                var context = new PlayContext(executingPlayer, cardPlayed);

                return new PlayCommand(rules, gameState, context);
            }
        }

        public class IsValidShould
        {
            private readonly Player player1;
            private readonly Player player2;
            private readonly Player player3;
            private readonly Player player4;

            public IsValidShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player2 = sampleData.Player2;
                player3 = sampleData.Player3;
                player4 = sampleData.Player4;
            }

            [Fact]
            public void ReturnIsValidFalse_WhenCardPlayedIsNotInPlayersHand()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection();
                var player2Hand = new CardCollection(Card(1, Clubs));
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithExecutingPlayer(player1)
                    .Build(cardPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnIsValidFalse_WhenNotPlayersTurn()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection();
                var player2Hand = new CardCollection(
                    cardPlayed
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player2)
                    .Build(cardPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnIsValidFalse_WhenCardPlayedNotTrickSuit_AndPlayerHasCardOfTrickSuit()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(1, Diamonds));
                var player2Hand = new CardCollection();
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .WithPreviousPlays(ImmutableList.Create(
                        new Play(player4, Card(13, Clubs))))
                    //.WithPreviousPlays(Cards(Card(13, Diamonds)))
                    .Build(cardPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.InvalidPlay);
            }            
            
            [Fact]
            public void ReturnIsValidTrue_WhenFirstCardPlayedInTrick()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(1, Diamonds));
                var player2Hand = new CardCollection();
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .Build(cardPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsValidTrue_WhenCardPlayedMatchesTrickSuit()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(1, Diamonds));
                var player2Hand = new CardCollection();
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .WithPreviousPlays(ImmutableList.Create(
                        new Play(player4, Card(13, Clubs))))
                    //.WithPreviousPlays(Cards(Card(13, Clubs)))
                    .Build(cardPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsValidTrue_WhenPlayerDoesNotHaveCardMatchingTrickSuit()
            {
                var cardPlayed = Card(1, Diamonds);
                var player1Hand = new CardCollection(
                    Card(1, Diamonds));
                var player2Hand = new CardCollection();
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .WithPreviousPlays(ImmutableList.Create(
                        new Play(player4, Card(13, Clubs))))
                    //.WithPreviousPlays(Cards(Card(13, Clubs)))
                    .Build(cardPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeTrue();
            }
        }

        public class ExecuteShould
        {
            private readonly Player player1;
            private readonly Player player2;
            private readonly Player player3;
            private readonly Player player4;

            public ExecuteShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player2 = sampleData.Player2;
                player3 = sampleData.Player3;
                player4 = sampleData.Player4;
            }

            [Fact]
            public void ReturnUpdatedTable()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs)
                    );
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .Build(cardPlayed);

                var actual = sut.Execute();

                actual.CurrentTable.Trick.CardCollection.Cards.First().Should().Be(cardPlayed);
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(Card(1, Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().Contain(Card(2, Clubs));
            }

            [Fact]
            public void CompleteTrick()
            {
                var cardPlayed = Card(5, Clubs);
                var player1Hand = new CardCollection(Card(2, Clubs));
                var player2Hand = new CardCollection(Card(3, Clubs));
                var player3Hand = new CardCollection(Card(5, Clubs));
                var player4Hand = new CardCollection(Card(4, Clubs));
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, player3Hand)
                    .WithPlayer4(player4, player4Hand)
                    .WithPlayerToPlay(player4)
                    .WithExecutingPlayer(player4)
                    .WithPreviousPlays(ImmutableList.Create(
                        new Play(player1, Card(2, Clubs)),
                        new Play(player2, Card(3, Clubs)),
                        new Play(player3, Card(4, Clubs))))
                    //.WithPreviousPlays(Cards(Card(2, Clubs), Card(3, Clubs), Card(4, Clubs)))
                    .Build(cardPlayed);

                var actual = sut.Execute();

                actual.CurrentTable.Trick.CardCollection.Cards.Should()
                    .Equal(Card(2, Clubs), Card(3, Clubs), Card(4, Clubs), Card(5, Clubs));
                actual.CurrentStateOfTrick.HasWinner.Should().BeTrue();
                //actual.CurrentStateOfTrick.Winner.Should().Be(player3);
            }

            [Fact]
            public void AddTrickStartedEvent()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs)
                    );
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .Build(cardPlayed);

                var actual = sut.Execute();

                var actualEvent = actual.EventLog.Events.First();
                actualEvent.Should().BeOfType<TrickStarted>();
                var domainEvent = actualEvent as TrickStarted;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(1);
            }

            [Fact]
            public void AddPlayedEvent()
            {
                var cardPlayed = Card(1, Clubs);
                var player1Hand = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Clubs)
                    );
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, new CardCollection())
                    .WithPlayer4(player4, new CardCollection())
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .Build(cardPlayed);

                var actual = sut.Execute();

                var actualEvent = actual.EventLog.Events.FirstSkip(1);
                actualEvent.Should().BeOfType<Played>();
                var playedEvent = actualEvent as Played;
                if (playedEvent == null) Assert.NotNull(playedEvent);
                playedEvent.Number.Should().Be(2);
                playedEvent.PlayerNumber.Should().Be(1);
                playedEvent.Cards.Should().Equal(cardPlayed);
            }
            

            [Fact]
            public void AddTrickCompletedEvent()
            {
                var cardPlayed = Card(5, Clubs);
                var player1Hand = new CardCollection(Card(2, Clubs));
                var player2Hand = new CardCollection(Card(3, Clubs));
                var player3Hand = new CardCollection(Card(4, Clubs));
                var player4Hand = new CardCollection(Card(5, Clubs));
                var sut = new PlayCommandBuilder()
                    .WithPlayer1(player1, player1Hand)
                    .WithPlayer2(player2, player2Hand)
                    .WithPlayer3(player3, player3Hand)
                    .WithPlayer4(player4, player4Hand)
                    .WithPlayerToPlay(player4)
                    .WithExecutingPlayer(player4)
                    .WithPreviousPlays(ImmutableList.Create(
                            new Play(player1, Card(2, Clubs)), 
                            new Play(player2, Card(3, Clubs)),
                            new Play(player3, Card(4, Clubs))))
                    .Build(cardPlayed);

                var actual = sut.Execute();

                var actualEvent = actual.EventLog.Events.Last();
                actualEvent.Should().BeOfType<TrickCompleted>();
                var domainEvent = actualEvent as TrickCompleted;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(2);
                domainEvent.WinnerPlayerNumber.Should().Be(4);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using SheddingCardGames.Domain.Whist;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
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
            private ImmutableList<Card> cardsPlayed;

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
            
            public PlayCommandBuilder WithCardsPlayed(ImmutableList<Card> withCardsPlayed)
            {
                cardsPlayed = withCardsPlayed;
                return this;
            }
            
            public PlayCommand Build(Card cardPlayed)
            {
                playerToPlay ??= players.First();
                executingPlayer ??= players.First();

                var rules = new Rules();
                var gameState = new GameState
                {
                    CurrentStateOfPlay = new StateOfPlay(),
                    CurrentStateOfTrick = new StateOfTrick(1, players.First(), playerToPlay, cardsPlayed)
                };

                var context = new PlayContext(executingPlayer, cardPlayed);

                return new PlayCommand(rules, gameState, context);
            }
        }

        public class IsValidShould
        {
            private readonly Player player1;
            private readonly Player player2;

            public IsValidShould()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player2 = sampleData.Player2;
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
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .WithCardsPlayed(Cards(Card(13, Diamonds)))
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
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .WithCardsPlayed(Cards(Card(13, Clubs)))
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
                    .WithPlayerToPlay(player1)
                    .WithExecutingPlayer(player1)
                    .WithCardsPlayed(Cards(Card(13, Clubs)))
                    .Build(cardPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeTrue();
            }
        }
    }
}

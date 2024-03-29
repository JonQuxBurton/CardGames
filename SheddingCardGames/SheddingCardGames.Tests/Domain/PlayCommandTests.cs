﻿using System.Collections.Immutable;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules.NumberOfPlayers;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain
{
    namespace PlayCommandTests
    {
        public class PlayCommandBuilder
        {
            private DiscardPile discardPile = new DiscardPile();
            private int executingPlayerNumber = 1;
            private CardCollection player1Hand = new CardCollection();
            private CardCollection player2Hand = new CardCollection();
            private readonly int turnNumber = 1;
            private Suit? selectedSuit;
            private IImmutableList<Action> previousActions = ImmutableList.Create<Action>();
            private int? winnerPlayerNumber;

            public PlayCommandBuilder WithExecutingPlayer(int withExecutingPlayerNumber)
            {
                executingPlayerNumber = withExecutingPlayerNumber;
                return this;
            }

            public PlayCommandBuilder WithPlayer1Hand(CardCollection withPlayer1Hand)
            {
                player1Hand = withPlayer1Hand;
                return this;
            }

            public PlayCommandBuilder WithPlayer2Hand(CardCollection withPlayer2Hand)
            {
                player2Hand = withPlayer2Hand;
                return this;
            }

            public PlayCommandBuilder WithSelectedSuit(Suit withSelectedSuit)
            {
                selectedSuit = withSelectedSuit;
                previousActions = ImmutableList.Create(Action.Play);
                return this;
            }

            public PlayCommandBuilder WithDiscardPile(DiscardPile withDiscardPile)
            {
                discardPile = withDiscardPile;
                return this;
            }

            public PlayCommandBuilder WithWinner(int withWinnerPlayerNumber)
            {
                winnerPlayerNumber = withWinnerPlayerNumber;
                return this;
            }

            public PlayCommand Build(ImmutableList<Card> playedCards)
            {
                var sampleData = new SampleData();
                var player1 = sampleData.Player1;
                player1.Hand = player1Hand;
                var player2 = sampleData.Player2;
                player2.Hand = player2Hand;
                var currentPlayer = player1;
                var executingPlayer = player1;
                if (executingPlayerNumber == 2)
                    executingPlayer = player2;

                Player winner = null;

                if (winnerPlayerNumber.HasValue)
                    winner = sampleData.GetPlayer(winnerPlayerNumber.Value);

                discardPile.TurnUpTopCard();
                var players = Players(sampleData.Player1, sampleData.Player2);
                var table = TableCreator.Create(new StockPile(new CardCollection()), discardPile, players);
                var gameState = new GameState
                {
                    GameSetup = new GameSetup(players)
                };
                gameState.GameSetup.WithStartingPlayer(currentPlayer);
                gameState.CurrentTable = table;
                gameState.CurrentStateOfTurn = new StateOfTurn(turnNumber, currentPlayer, Action.Play, null, selectedSuit, previousActions);
                gameState.CurrentStateOfPlay = new StateOfPlay();
                if (winner != null)
                    gameState.CurrentStateOfPlay = StateOfPlay.WithWinner(gameState.CurrentStateOfPlay , winner);

                return new PlayCommand(new BasicVariantRules(Two), gameState,  new PlayContext(executingPlayer, playedCards));
            }
        }

        public class IsValidShould
        {
            [Fact]
            public void ReturnIsValidTrueWhenValid()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Clubs)
                ));
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeTrue();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.Success);
            }

            [Fact]
            public void ReturnIsValidFalseWhenCardIsNotInPlayersHand()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection();
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.CardIsNotInPlayersHand);
            }

            [Fact]
            public void ReturnIsValidFalseWhenNotPlayersTurn()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var discardPile = new DiscardPile(
                    new[] { Card(1, Clubs) }
                );
                var player1Hand = new CardCollection();
                var player2Hand = new CardCollection(
                    cardsPlayed
                );
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithExecutingPlayer(2)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.NotPlayersTurn);
            }

            [Fact]
            public void ReturnIsValidFalseForInvalidPlay()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void ReturnIsValidTrueForValidPlayWithMatchingSuit()
            {
                var cardPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardPlayed);

                var actual = sut.IsValid();
                actual.IsValid.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsValidTrueForValidPlayWithMatchingSuitWhenSelectedSuit()
            {
                var playedCards = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(playedCards);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(playedCards);

                var actual = sut.IsValid();
                actual.IsValid.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsValidFalse_WhenCardPlayed_AndSelectedSuit_ButDoesNotMatchSuit()
            {
                var selectedSuit = Hearts;
                var playedCards = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(playedCards);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithSelectedSuit(selectedSuit)
                    .Build(playedCards);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
            }

            [Fact]
            public void ReturnIsValidTrueForValidPlayWithRank8()
            {
                var cardsPlayed = Cards(Card(8, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsValidTrueForAnyCardWhenFirstTurnAndDiscardCardIs8()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();
                actual.IsValid.Should().BeTrue();
            }

            [Fact]
            public void ReturnIsValidFalseWhenFirstTurnAndDiscardCardIsNot8()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(7, Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.InvalidPlay);
            }

            [Fact]
            public void ReturnIsValidFalse_WhenGameHasBeenWon()
            {
                var cardsPlayed = Cards(Card(1, Clubs));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Clubs)
                ));
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithDiscardPile(discardPile)
                    .WithWinner(1)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.GameCompleted);
            }
        }
        
        public class IsValid_WhenMultipleCardsPlayed_Should
        {
            private readonly CardParser cardParser = new CardParser();

            [Theory]
            [InlineData("2|Hearts", "2|Clubs", "2|Diamonds")]
            [InlineData("2|Clubs", "2|Hearts", "2|Diamonds")]
            [InlineData("2|Clubs", "2|Diamonds", "2|Hearts")]
            public void ReturnIsValidFalse_WhenAnyCardIsNotInPlayersHand(params string[] playedCardsData)
            {
                var cardsPlayed = Cards(cardParser.Parse(playedCardsData));
                var player1Hand = new CardCollection(
                    Card(2, Clubs),
                    Card(2, Diamonds)
                    );
                var discardPile = new DiscardPile(
                    new[] { Card(1, Clubs) }
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.CardIsNotInPlayersHand);
            }
            
            [Theory]
            [InlineData("2|Clubs", "2|Diamonds", "10|Hearts")]
            [InlineData("2|Clubs", "10|Hearts", "2|Diamonds")]
            [InlineData("10|Hearts", "2|Clubs", "2|Diamonds")]
            [InlineData("2|Clubs", "2|Diamonds", "10|Spades")]
            public void ReturnIsValidFalse_ForInvalidPlay(params string[] playedCardsData)
            {
                var cardsPlayed = Cards(cardParser.Parse(playedCardsData));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(2, Spades)
                ));
                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();

                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.InvalidPlay);
            }

            [Theory]
            [InlineData("1|Clubs", "1|Diamonds")]
            [InlineData("1|Clubs", "1|Diamonds", "1|Hearts")]
            [InlineData("10|Spades", "10|Clubs")]
            public void ReturnIsValidTrue_ForValidPlayWithMatchingRank(params string[] playedCardsData)
            {
                var cardsPlayed = Cards(cardParser.Parse(playedCardsData));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();
                actual.IsValid.Should().BeTrue();
            }
            
            [Theory]
            [InlineData("1|Clubs", "1|Diamonds")]
            [InlineData("1|Clubs", "1|Diamonds", "1|Hearts")]
            [InlineData("1|Clubs", "1|Diamonds", "1|Hearts", "1|Spades")]
            public void ReturnIsValidTrue_ForAnyCardsWithMatchingRank_WhenFirstTurn_AndDiscardCardIs8(params string[] playedCardsData)
            {
                var cardsPlayed = Cards(cardParser.Parse(playedCardsData));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();
                actual.IsValid.Should().BeTrue();
            }

            [Theory]
            [InlineData("1|Clubs", "2|Spades")]
            [InlineData("1|Clubs", "8|Clubs")]
            [InlineData("1|Clubs", "1|Diamonds", "2|Spades")]
            [InlineData("1|Clubs", "1|Diamonds", "1|Hearts", "2|Spades")]
            [InlineData("1|Clubs", "1|Diamonds", "1|Hearts", "1|Spades", "2|Spades")]
            public void ReturnIsValidFalse_WhenAnyCardsDoesNotMatchRankOfFirstCard_WhenFirstTurn_AndDiscardCardIs8(params string[] playedCardsData)
            {
                var cardsPlayed = Cards(cardParser.Parse(playedCardsData));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(8, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();
                actual.IsValid.Should().BeFalse();
            }

            [Fact]
            public void ReturnIsValidFalse_WhenGameWon()
            {
                var cardsPlayed = Cards(Card(1, Clubs), Card(1, Diamonds), Card(1, Hearts));
                var player1Hand = new CardCollection(cardsPlayed);
                var discardPile = new DiscardPile(new CardCollection(
                    Card(1, Spades)
                ));

                var sut = new PlayCommandBuilder()
                    .WithDiscardPile(discardPile)
                    .WithPlayer1Hand(player1Hand)
                    .WithWinner(1)
                    .Build(cardsPlayed);

                var actual = sut.IsValid();
                actual.IsValid.Should().BeFalse();
                actual.MessageKey.Should().Be(CommandIsValidResultMessageKey.GameCompleted);
            }
        }

        public class ExecuteShould
        {
            [Fact]
            public void ReturnUpdatedTable()
            {
                var cardsPlayed = Cards(
                    Card(1, Clubs), 
                    Card(1, Diamonds)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                ));
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var discardPile = new DiscardPile(new [] { Card(1, Hearts) });
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(Card(1, Diamonds));
                actual.CurrentTable.DiscardPile.RestOfCards.Cards.First().Should().Be(Card(1, Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(Card(1, Clubs));
                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(Card(1, Diamonds));
            }

            [Fact]
            public void AddPlayedEvent()
            {
                var cardsPlayed = Cards(
                    Card(1, Clubs),
                    Card(1, Diamonds)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                    ));
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var discardPile = new DiscardPile(new[] { Card(1, Hearts) });
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                actual.EventLog.Events.LastSkip(1).Should().BeOfType<Played>();
                var actualEvent = actual.EventLog.Events.LastSkip(1) as Played;
                if (actualEvent == null) Assert.NotNull(actualEvent);
                actualEvent.Number.Should().Be(1);
                actualEvent.PlayerNumber.Should().Be(1);
                actualEvent.Cards.Should().Equal(cardsPlayed);
            }

            [Fact]
            public void AddTurnEndedEvent()
            {
                var cardsPlayed = Cards(
                    Card(1, Clubs),
                    Card(1, Diamonds)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                    ));
                var player2Hand = new CardCollection(
                    Card(2, Diamonds)
                );
                var discardPile = new DiscardPile(new[] { Card(1, Hearts) });
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                var actualEvent = actual.EventLog.Events.Last();
                actualEvent.Should().BeOfType<TurnEnded>();
                var domainEvent = actualEvent as TurnEnded;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.Number.Should().Be(2);
                domainEvent.PlayerNumber.Should().Be(1);
            }

            [Fact]
            public void CreateNewTurn()
            {
                var cardsPlayed = Cards
                (
                    Card(1, Clubs),
                    Card(1, Diamonds)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                    ));
                var player2Hand = new CardCollection(
                    Card(2, Diamonds),
                    Card(3, Diamonds)
                );
                var discardPile = new DiscardPile(new[] { Card(1, Hearts) });
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .WithDiscardPile(discardPile)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentStateOfTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.CurrentAction.Should().Be(Action.Play);

                actual.CurrentStateOfPlay.HasWinner.Should().BeFalse();
                actual.CurrentStateOfPlay.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().BeNull();
                actualTurn.TakenCard.Should().BeNull();

                actual.CurrentTable.Players[0].Hand.Cards.Should().NotContain(cardsPlayed);
                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardsPlayed.Last());

                actualTurn.PreviousActions.Should().BeEmpty();
            }

            [Fact]
            public void CreateNewTurnWithNextActionTake()
            {
                var cardsPlayed = Cards (
                    Card(1, Clubs)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                        Card(2, Clubs)
                    )
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentStateOfTurn;
                actualTurn.TurnNumber.Should().Be(2);
                actualTurn.PlayerToPlay.Number.Should().Be(2);
                actualTurn.CurrentAction.Should().Be(Action.Take);

                actual.CurrentStateOfPlay.HasWinner.Should().BeFalse();
                actual.CurrentStateOfPlay.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().BeNull();
                actualTurn.TakenCard.Should().BeNull();

                actualTurn.PreviousActions.Should().BeEmpty();
            }

            [Fact]
            public void CreateWinningTurn()
            {
                var cardsPlayed = Cards(
                    Card(1, Clubs)
                );
                var player1Hand = new CardCollection(
                    cardsPlayed
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentStateOfTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.CurrentAction.Should().Be(Action.Won);

                actual.CurrentStateOfPlay.HasWinner.Should().BeTrue();
                actual.CurrentStateOfPlay.Winner.Number.Should().Be(1);
                actualTurn.SelectedSuit.Should().BeNull();
                actualTurn.TakenCard.Should().BeNull();

                actualTurn.PreviousActions.Should().Equal(Action.Play);

                var actualEvent = actual.EventLog.Events.Last();
                actualEvent.Should().BeOfType<RoundWon>();
                var domainEvent = actualEvent as RoundWon;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
            }

            [Fact]
            public void CreateCrazyEightTurn()
            {
                var cardsPlayed = Cards(Card(8, Clubs));
                var player1Hand = new CardCollection(
                    cardsPlayed.Append(
                    Card(1, Clubs)
                    )
                );
                var player2Hand = new CardCollection(
                    Card(10, Diamonds)
                );
                var sut = new PlayCommandBuilder()
                    .WithPlayer1Hand(player1Hand)
                    .WithPlayer2Hand(player2Hand)
                    .Build(cardsPlayed);

                var actual = sut.Execute();

                var actualTurn = actual.CurrentStateOfTurn;
                actualTurn.TurnNumber.Should().Be(1);
                actualTurn.PlayerToPlay.Number.Should().Be(1);
                actualTurn.CurrentAction.Should().Be(Action.SelectSuit);

                actual.CurrentStateOfPlay.HasWinner.Should().BeFalse();
                actual.CurrentStateOfPlay.Winner.Should().BeNull();
                actualTurn.SelectedSuit.Should().BeNull();
                actualTurn.TakenCard.Should().BeNull();

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardsPlayed.First());

                actual.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardsPlayed.First());

                var actualEvent = actual.EventLog.Events.Last();
                actualEvent.Should().BeOfType<Played>();
                var domainEvent = actualEvent as Played;
                if (domainEvent == null) Assert.NotNull(domainEvent);
                domainEvent.PlayerNumber.Should().Be(1);
            }
        }
    }
}
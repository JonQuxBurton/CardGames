using System.Collections.Immutable;
using FluentAssertions;
using SheddingCardGames.Domain;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.PlayersUtils;

namespace SheddingCardGames.Tests.Domain
{
    namespace GameStateTests
    {
        public class NextPlayerShould
        {
            private readonly SampleData sampleData;

            public NextPlayerShould()
            {
                sampleData = new SampleData();
            }

            private GameState CreateSut(Player playerToPlay)
            {
                var players = Players(sampleData.Player1, sampleData.Player2);
                
                return new GameState()
                {
                    GameSetup = new GameSetup(players),
                    CurrentTable = new Table(new StockPile(new CardCollection()), new DiscardPile(), players),
                    CurrentStateOfTurn = new StateOfTurn(1, playerToPlay, Action.Play)
                };
            }

            [Fact]
            public void ReturnNextPlayer()
            {
                var sut = CreateSut(sampleData.Player1);

                var actual = sut.NextPlayer;

                actual.Should().Be(sampleData.Player2);
            }

            [Fact]
            public void ReturnFirstPlayerAfterLastPlayer()
            {
                var sut = CreateSut(sampleData.Player2);

                var actual = sut.NextPlayer;

                actual.Should().Be(sampleData.Player1);
            }
        }

        public class AnyPlaysOrTakesShould
        {
            private readonly SampleData sampleData;
            private readonly ImmutableList<Player> players;

            public AnyPlaysOrTakesShould()
            {
                sampleData = new SampleData();
                players = Players(sampleData.Player1, sampleData.Player2);
            }

            [Fact]
            public void ReturnFalse_WhenNoCurrentTurn()
            {
                var gameState = new GameState
                {
                    GameSetup = new GameSetup(players),
                    CurrentTable = new Table(new StockPile(new CardCollection()), new DiscardPile(), players)
                };
                var sut = new StateOfPlay(gameState);

                var actual = sut.AnyPlaysOrTakes;

                actual.Should().BeFalse();
            }

            [Fact]
            public void ReturnTrue_WhenNotFirstTurn()
            {
                var gameState = new GameState
                {
                    GameSetup = new GameSetup(players),
                    CurrentTable = new Table(new StockPile(new CardCollection()), new DiscardPile(), players),
                    CurrentStateOfTurn = new StateOfTurn(2, sampleData.Player2, Action.Play)
                };
                var sut = new StateOfPlay(gameState);

                var actual = sut.AnyPlaysOrTakes;

                actual.Should().BeTrue();
            }

            [Fact]
            public void ReturnTrue_WhenEightPlayed_ButSuitHasNotBeenSelected()
            {
                var discardPile = new DiscardPile(Cards(Card(8, Suit.Clubs)));
                discardPile.TurnUpTopCard();

                var gameState = new GameState
                {
                    GameSetup = new GameSetup(players),
                    CurrentTable = new Table(new StockPile(new CardCollection()), discardPile, players),
                    CurrentStateOfTurn = new StateOfTurn(1, sampleData.Player1, Action.SelectSuit, null, null, ImmutableList.Create(Action.Play))
                };
                var sut = new StateOfPlay(gameState);

                var actual = sut.AnyPlaysOrTakes;

                actual.Should().BeTrue();
            }

        }
    }
}
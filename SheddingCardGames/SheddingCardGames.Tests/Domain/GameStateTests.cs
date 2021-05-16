using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Xunit;
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
                return new GameState(players)
                {
                    CurrentTable = new Table(new StockPile(new CardCollection()), new DiscardPile(), players),
                    CurrentTurn = new CurrentTurn(1, playerToPlay, Action.Play)
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
    }
}
using System.Collections.Immutable;
using FluentAssertions;
using Moq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules.NumberOfPlayers;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain
{
    namespace OlsenOlsenCommandFactoryTests
    {
        public class CreateShould
        {
            private readonly SampleData sampleData;
            private readonly OlsenOlsenVariantCommandFactory sut;
            private readonly GameState gameState;
            private readonly ImmutableList<Player> players;

            public CreateShould()
            {
                sampleData = new SampleData();
                players = Players(sampleData.Player1, sampleData.Player2);
                gameState = new GameState(players);
                sut = new OlsenOlsenVariantCommandFactory(new CrazyEightsRules(Two), new DummyShuffler());
            }

            [Fact]
            public void ReturnChooseStartingPlayerCommand()
            {
                var actual = sut.Create(gameState, new ChooseStartingPlayerContext(sampleData.Player1));

                actual.Should().BeOfType<ChooseStartingPlayerCommand>();
            }

            [Fact]
            public void ReturnDealCommand()
            {
                var actual = sut.Create(gameState,
                    new DealContext(new DeckBuilder().Build()));

                actual.Should().BeOfType<DealCommand>();
            }

            [Fact]

            public void ReturnPlayCommand()
            {
                var actual = sut.Create(gameState, new PlayContext(sampleData.Player1, Cards(Card(1, Clubs))));

                actual.Should().BeOfType<PlayCommand>();
            }

            [Fact]
            public void ReturnSelectSuitCommand()
            {
                var actual = sut.Create(gameState, new SelectSuitContext(sampleData.Player1, Clubs));

                actual.Should().BeOfType<SelectSuitCommand>();
            }

            [Fact]
            public void ReturnTakeCommand()
            {
                var actual = sut.Create(gameState, new TakeContext(sampleData.Player1));

                actual.Should().BeOfType<TakeCommand>();
            }

            [Fact]
            public void ReturnNullForUnknownCommand()
            {
                var invalidContext = new Mock<ICommandContext>();
                var actual = sut.Create(gameState, invalidContext.Object);

                actual.Should().BeNull();
            }
        }
    }
}
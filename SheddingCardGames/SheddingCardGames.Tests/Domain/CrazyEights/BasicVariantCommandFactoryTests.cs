using FluentAssertions;
using Moq;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEights.CrazyEightsRules;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain.CrazyEights
{
    namespace BasicCommandFactoryTests
    {
        public class CreateShould
        {
            private readonly SampleData sampleData;
            private readonly BasicVariantCommandFactory sut;
            private readonly GameState gameState;

            public CreateShould()
            {
                sampleData = new SampleData();
                sut = new BasicVariantCommandFactory(new BasicVariantRules(NumberOfPlayers.Two), new DummyShuffler(), new RandomPlayerChooser());
                var players = Players(sampleData.Player1, sampleData.Player2);
                gameState = new GameState
                {
                    GameSetup = new GameSetup(players)
                };

            }

            [Fact]
            public void ReturnChooseStartingPlayerCommand()
            {
                var actual = sut.Create(gameState, new ChooseStartingPlayerContext());

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

            public void ReturnPlaySingleCommand()
            {
                var actual = sut.Create(gameState, new PlayContext(sampleData.Player1, Cards(Card(1, Clubs))));

                actual.Should().BeOfType<PlaySingleCommand>();
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
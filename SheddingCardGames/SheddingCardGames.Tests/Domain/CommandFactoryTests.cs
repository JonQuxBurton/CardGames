﻿using FluentAssertions;
using Moq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace CommandFactoryTests
    {
        public class CreateShould
        {
            private readonly SampleData sampleData;
            private readonly CommandFactory sut;

            public CreateShould()
            {
                sampleData = new SampleData();
                sut = new CommandFactory(new Rules(), new DummyShuffler());
            }

            [Fact]
            public void ReturnChooseStartingPlayerCommand()
            {
                var actual = sut.Create(new GameState(), new ChooseStartingPlayerContext(sampleData.Player1));

                actual.Should().BeOfType<ChooseStartingPlayerCommand>();
            }

            [Fact]
            public void ReturnDealCommand()
            {
                var actual = sut.Create(new GameState(),
                    new DealContext(new DeckBuilder().Build(), new[] {sampleData.Player1, sampleData.Player2}));

                actual.Should().BeOfType<DealCommand>();
            }

            [Fact]
            public void ReturnPlayCommand()
            {
                var actual = sut.Create(new GameState(), new PlayContext(sampleData.Player1, new Card(1, Suit.Clubs)));

                actual.Should().BeOfType<PlayCommand>();
            }

            [Fact]
            public void ReturnSelectSuitCommand()
            {
                var actual = sut.Create(new GameState(), new SelectSuitContext(sampleData.Player1, Suit.Clubs));

                actual.Should().BeOfType<SelectSuitCommand>();
            }

            [Fact]
            public void ReturnTakeCommand()
            {
                var actual = sut.Create(new GameState(), new TakeContext(sampleData.Player1));

                actual.Should().BeOfType<TakeCommand>();
            }

            [Fact]
            public void ReturnNullForUnknownCommand()
            {
                var invalidContext = new Mock<ICommandContext>();
                var actual = sut.Create(new GameState(), invalidContext.Object);

                actual.Should().BeNull();
            }
        }
    }
}
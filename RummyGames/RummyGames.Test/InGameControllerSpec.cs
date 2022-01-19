using System;
using System.Linq;
using CardGamesDomain;
using FluentAssertions;
using Xunit;

namespace RummyGames.Test
{
    public class InGameControllerSpec
    {
        private readonly Player guest;
        private readonly Player host;

        public InGameControllerSpec()
        {
            host = new Player(Guid.NewGuid(), "Alice");
            guest = new Player(Guid.NewGuid(), "Bob");
        }

        [Fact]
        public void ShuffleDeck_Should_ShuffleDeck()
        {
            var deckBuilder = new DeckBuilder();
            var expected = deckBuilder.Build().Cards.Reverse().ToList();
            var inGameState = new InGameState(Guid.NewGuid(), new Table(new[]{ host, guest }, deckBuilder.Build()), host);
            var shuffler = new FakeShuffler(expected);
            var sut = new InGameController(shuffler);

            var actual = sut.ShuffleDeck(inGameState);

            actual.Table.Deck.Cards.Should().Equal(expected);
        }
    }
}
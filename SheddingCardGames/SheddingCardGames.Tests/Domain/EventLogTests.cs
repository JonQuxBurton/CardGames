using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;
using Xunit;

namespace SheddingCardGames.Tests.Domain
{
    namespace EventLogTests
    {
        public class AddEventShould
        {
            [Fact]
            public void AddEventsToEventLog()
            {
                var sampleData = new SampleData();
                var deck = new DeckBuilder().Build();

                var sut = new EventLog();

                sut.AddEvent(new StartingPlayerChosen(1, sampleData.Player1));
                sut.AddEvent(new Shuffled(2, CardMoveSources.StockPile, deck, deck));
                sut.AddEvent(new DealCompleted(3));

                var event1 = sut.Events.ElementAt(0);
                event1.Should().BeOfType<StartingPlayerChosen>();
                event1.Number.Should().Be(1);

                var event2 = sut.Events.ElementAt(1);
                event2.Should().BeOfType<Shuffled>();
                event2.Number.Should().Be(2);

                var event3 = sut.Events.ElementAt(2);
                event3.Should().BeOfType<DealCompleted>();
                event3.Number.Should().Be(3);
            }
        }

        public class NextEventNumberShould
        {
            [Fact]
            public void ReturnNextEventNumber()
            {
                var sampleData = new SampleData();
                var deck = new DeckBuilder().Build();

                var sut = new EventLog();

                sut.AddEvent(new StartingPlayerChosen(1, sampleData.Player1));
                sut.AddEvent(new Shuffled(2, CardMoveSources.StockPile, deck, deck));
                sut.AddEvent(new DealCompleted(3));

                sut.NextEventNumber.Should().Be(4);
            }
            
            [Fact]
            public void ReturnOne_WhenEmpty()
            {
                var sut = new EventLog();

                sut.NextEventNumber.Should().Be(1);
            }
        }
    }
}
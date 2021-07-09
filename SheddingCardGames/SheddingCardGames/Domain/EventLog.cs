using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain
{
    public class EventLog
    {
        private readonly List<DomainEvent> events = new List<DomainEvent>();
        public IEnumerable<DomainEvent> Events => events;
        public int NextEventNumber => Events.Select(x => x.Number).DefaultIfEmpty().Max() + 1;

        public void AddEvent(DomainEvent newEvent)
        {
            events.Add(newEvent);
        }
    }
}
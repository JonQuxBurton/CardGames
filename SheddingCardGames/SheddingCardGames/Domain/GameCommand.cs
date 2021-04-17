using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public abstract class GameCommand
    {
        public abstract ActionResult IsValid();
        public abstract GameState Execute();

        protected int GetNextEventNumber(List<DomainEvent> events)
        {
            return events.Select(x => x.Number).DefaultIfEmpty().Max() + 1;
        }
    }
}
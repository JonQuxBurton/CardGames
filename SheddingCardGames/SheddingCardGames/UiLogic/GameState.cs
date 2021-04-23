using System.Collections.Generic;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.UiLogic
{
    public class GameState
    {
        public Table CurrentTable { get; set; }
        public List<DomainEvent> Events { get; set; } = new List<DomainEvent>();

        public GamePhase CurrentGamePhase { get; set; }
        public CurrentTurn CurrentTurn { get; set; }
        public PreviousTurnResult PreviousTurnResult { get; set; }
        public Player PlayerToStart { get; set; }
    }
}
using System.Collections.Generic;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.UiLogic
{
    public class GameState
    {
        public Table CurrentTable { get; set; }
        public List<DomainEvent> Events { get; set; } = new List<DomainEvent>();
        public Suit? SelectedSuit { get; set; }
        public int TurnNumber { get; set; }
        public Card TakenCard { get; set; }
        public Player PlayerToPlay { get; set; }
        public CurrentTurn CurrentTurn { get; set; }
        public GamePhase CurrentGamePhase { get; set; }

    }
}
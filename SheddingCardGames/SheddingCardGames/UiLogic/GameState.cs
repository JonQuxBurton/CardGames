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

        public Player NextPlayer
        {
            get
            {
                var nextPlayerNumber = CurrentTurn.PlayerToPlay.Number + 1;
                if (nextPlayerNumber > CurrentTable.Players.Count)
                    nextPlayerNumber = 1;

                return CurrentTable.Players[nextPlayerNumber - 1];
            }
        }
    }
}
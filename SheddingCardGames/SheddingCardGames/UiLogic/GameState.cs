using System.Collections.Generic;
using System.Linq;
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

        public int CurrentTurnNumber => CurrentTurn.TurnNumber;
        public Player CurrentPlayerToPlay => CurrentTurn.PlayerToPlay;
        public int CurrentPlayerToPlayNumber => CurrentTurn.PlayerToPlay.Number;
        public Card CurrentCardToMatch => CurrentTable.DiscardPile.CardToMatch;
        public Suit? CurrentSelectedSuit => PreviousTurnResult?.SelectedSuit;

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

        public int NextEventNumber =>  Events.Select(x => x.Number).DefaultIfEmpty().Max() + 1;

    }
}
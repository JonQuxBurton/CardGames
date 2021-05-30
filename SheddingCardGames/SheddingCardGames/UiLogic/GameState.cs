using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.UiLogic
{
    public class GameState
    {
        public IImmutableList<Player> Players { get; }
        private readonly List<DomainEvent> events = new List<DomainEvent>();

        public GameState(IImmutableList<Player> players)
        {
            Players = players;
        }

        public Table CurrentTable { get; set; }
        public IEnumerable<DomainEvent> Events => events;

        public GamePhase CurrentGamePhase { get; set; }
        public CurrentTurn CurrentTurn { get; set; }
        public bool AnyPlaysOrTakes
        {
            get
            {
                if (CurrentTurn == null)
                    return false;

                return CurrentTurn.TurnNumber != 1 || CurrentTurn.PreviousActions.Any();
            }
        }

        public Player PlayerToStart { get; set; }

        public int CurrentTurnNumber => CurrentTurn.TurnNumber;
        public Player CurrentPlayerToPlay => CurrentTurn.PlayerToPlay;
        public int CurrentPlayerToPlayNumber => CurrentTurn.PlayerToPlay.Number;
        public Card CurrentCardToMatch => CurrentTable.DiscardPile.CardToMatch;
        public Suit? CurrentSelectedSuit => CurrentTurn?.SelectedSuit;

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

        public int NextEventNumber => Events.Select(x => x.Number).DefaultIfEmpty().Max() + 1;

        public void AddEvent(DomainEvent newEvent)
        {
            events.Add(newEvent);
        }
    }
}
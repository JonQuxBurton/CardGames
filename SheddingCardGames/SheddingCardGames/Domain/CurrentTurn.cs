using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public class CurrentTurn
    {
        public CurrentTurn(int turnNumber,
            Player playerToPlay,
            Action currentAction, 
            Card takenCard = null,
            Suit? selectedSuit = null,
            Player winner = null,
            IImmutableList<Action> previousActions = null, 
            PreviousTurn previousTurn = null
        )
        {
            TurnNumber = turnNumber;
            PlayerToPlay = playerToPlay;
            CurrentAction = currentAction;
            TakenCard = takenCard;
            PreviousTurn = previousTurn;
            Winner = winner;
            SelectedSuit = selectedSuit;

            PreviousActions = previousActions ?? ImmutableList.Create<Action>();
        }

        public int TurnNumber { get; }
        public Player PlayerToPlay { get; }
        public Action CurrentAction { get; }
        public Suit? SelectedSuit { get; }
        public Card TakenCard { get; }

        public bool HasWinner => Winner != null;
        public Player Winner { get; }

        public PreviousTurn PreviousTurn { get; }
        public IImmutableList<Action> PreviousActions { get; }
    }

    public class PreviousTurn
    {
        public PreviousTurn(IImmutableList<Action> turnActions)
        {
            TurnActions = turnActions;
        }

        public IImmutableList<Action> TurnActions { get; }
    }
}
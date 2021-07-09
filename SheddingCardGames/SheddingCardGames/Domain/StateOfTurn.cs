using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public class StateOfTurn
    {
        public StateOfTurn(int turnNumber,
            Player playerToPlay,
            Action currentAction,
            Card takenCard = null,
            Suit? selectedSuit = null,
            IImmutableList<Action> previousActions = null)
        {
            TurnNumber = turnNumber;
            PlayerToPlay = playerToPlay;
            CurrentAction = currentAction;
            TakenCard = takenCard;
            SelectedSuit = selectedSuit;

            PreviousActions = previousActions ?? ImmutableList.Create<Action>();
        }

        public int TurnNumber { get; }
        public Player PlayerToPlay { get; }
        public Action CurrentAction { get; }
        public Card TakenCard { get; }
        public Suit? SelectedSuit { get; }
        public IImmutableList<Action> PreviousActions { get; }
    }
}
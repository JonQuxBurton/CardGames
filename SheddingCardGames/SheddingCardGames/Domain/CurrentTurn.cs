using System.Collections.Generic;

namespace SheddingCardGames.Domain
{
    public class CurrentTurn
    {
        public CurrentTurn(int turnNumber,
            Player playerToPlay,
            IEnumerable<Card> validPlays,
            bool hasWinner,
            Player winner,
            Action nextAction, 
            Suit? selectedSuit)
        {
            TurnNumber = turnNumber;
            PlayerToPlay = playerToPlay;
            ValidPlays = validPlays;
            HasWinner = hasWinner;
            Winner = winner;
            NextAction = nextAction;
            SelectedSuit = selectedSuit;
        }

        public int TurnNumber { get; }
        public Player PlayerToPlay { get; }
        public IEnumerable<Card> ValidPlays { get; }
        public bool HasWinner { get; }
        public Player Winner { get; }
        public Action NextAction { get; }
        public Suit? SelectedSuit { get; }
    }
}
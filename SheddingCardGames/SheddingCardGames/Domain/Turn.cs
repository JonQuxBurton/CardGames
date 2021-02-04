using System.Collections.Generic;

namespace SheddingCardGames.Domain
{
    public class Turn
    {
        public Turn(int turnNumber,
            int playerToPlay,
            IEnumerable<Card> validPlays,
            bool hasWinner,
            int? winner,
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
        public int PlayerToPlay { get; }
        public IEnumerable<Card> ValidPlays { get; }
        public bool HasWinner { get; }
        public int? Winner { get; }
        public Action NextAction { get; }
        public Suit? SelectedSuit { get; }
    }
}
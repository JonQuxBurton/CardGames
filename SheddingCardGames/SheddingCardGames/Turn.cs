using System.Collections.Generic;

namespace SheddingCardGames
{
    public enum Action
    {
        Play,
        SelectSuit,
        Take,
        Won
    }
    
    public class Turn
    {
        public Turn(int turnNumber,
            int playerToPlay,
            IEnumerable<Card> stockPile,
            DiscardPile discardPile,
            CardCollection player1Hand,
            CardCollection player2Hand,
            IEnumerable<Card> validPlays,
            bool hasWinner,
            int? winner,
            Action nextAction, 
            Suit? selectedSuit)
        {
            TurnNumber = turnNumber;
            PlayerToPlay = playerToPlay;
            StockPile = stockPile;
            DiscardPile = discardPile;
            Player1Hand = player1Hand;
            Player2Hand = player2Hand;
            ValidPlays = validPlays;
            HasWinner = hasWinner;
            Winner = winner;
            NextAction = nextAction;
            SelectedSuit = selectedSuit;
        }

        public int TurnNumber { get; }
        public int PlayerToPlay { get; }
        public IEnumerable<Card> StockPile { get; }
        public DiscardPile DiscardPile { get; }
        public IEnumerable<Card> ValidPlays { get; }
        public CardCollection Player1Hand { get; }
        public CardCollection Player2Hand { get; }
        public bool HasWinner { get; }
        public int? Winner { get; }
        public Action NextAction { get; }
        public Suit? SelectedSuit { get; }
    }
}
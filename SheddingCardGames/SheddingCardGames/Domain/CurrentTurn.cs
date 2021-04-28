namespace SheddingCardGames.Domain
{
    public class CurrentTurn
    {
        public CurrentTurn(int turnNumber,
            Player playerToPlay,
            ValidPlays validPlays,
            Action nextAction)
        {
            TurnNumber = turnNumber;
            PlayerToPlay = playerToPlay;
            ValidPlays = validPlays;
            NextAction = nextAction;
        }

        public int TurnNumber { get; }
        public Player PlayerToPlay { get; }
        public ValidPlays ValidPlays { get; }
        public Action NextAction { get; }
    }
}
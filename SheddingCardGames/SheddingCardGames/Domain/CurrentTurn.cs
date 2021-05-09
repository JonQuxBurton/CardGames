namespace SheddingCardGames.Domain
{
    public class CurrentTurn
    {
        public CurrentTurn(int turnNumber,
            Player playerToPlay,
            Action nextAction)
        {
            TurnNumber = turnNumber;
            PlayerToPlay = playerToPlay;
            NextAction = nextAction;
        }

        public int TurnNumber { get; }
        public Player PlayerToPlay { get; }
        public Action NextAction { get; }
    }
}
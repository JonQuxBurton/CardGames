namespace SheddingCardGames.Domain.Whist
{
    public class StateOfTrick
    {
        public StateOfTrick(int trickNumber, 
            Player playerToStart,
            Player playerToPlay)
        {
            TrickNumber = trickNumber;
            PlayerToStart = playerToStart;
            PlayerToPlay = playerToPlay;
        }


        public int TrickNumber { get; }
        public Player PlayerToStart { get; }
        public Player PlayerToPlay { get; }
    }
}
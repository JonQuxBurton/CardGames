namespace SheddingCardGames.Domain.CrazyEights
{
    public class TakeContext : ICommandContext
    {
        public TakeContext(Player executingPlayer, int numberOfPreviousTakes = 0)
        {
            ExecutingPlayer = executingPlayer;
            NumberOfPreviousTakes = numberOfPreviousTakes;
        }

        public Player ExecutingPlayer { get; }
        public int NumberOfPreviousTakes { get; }
    }
}
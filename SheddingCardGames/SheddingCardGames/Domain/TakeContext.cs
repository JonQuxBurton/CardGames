namespace SheddingCardGames.Domain
{
    public class TakeContext : ICommandContext
    {
        public TakeContext(Player executingPlayer)
        {
            ExecutingPlayer = executingPlayer;
        }

        public Player ExecutingPlayer { get; }
    }
}
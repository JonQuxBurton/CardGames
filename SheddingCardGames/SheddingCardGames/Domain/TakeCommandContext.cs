namespace SheddingCardGames.Domain
{
    public class TakeCommandContext
    {
        public TakeCommandContext(Player executingPlayer)
        {
            ExecutingPlayer = executingPlayer;
        }

        public Player ExecutingPlayer { get; }
    }
}
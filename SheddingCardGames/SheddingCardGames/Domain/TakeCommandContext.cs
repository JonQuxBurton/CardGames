namespace SheddingCardGames.Domain
{
    public class TakeCommandContext
    {
        public TakeCommandContext(Player player)
        {
            Player = player;
        }

        public Player Player { get; }
    }
}
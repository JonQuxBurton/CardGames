namespace SheddingCardGames.Domain.CrazyEights
{
    public class SelectSuitContext : ICommandContext
    {
        public SelectSuitContext(Player executingPlayer, Suit selectedSuit)
        {
            ExecutingPlayer = executingPlayer;
            SelectedSuit = selectedSuit;
        }

        public Player ExecutingPlayer { get; }
        public Suit SelectedSuit { get; }
    }
}
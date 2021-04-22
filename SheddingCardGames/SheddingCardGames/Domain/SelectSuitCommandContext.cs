namespace SheddingCardGames.Domain
{
    public class SelectSuitCommandContext
    {
        public SelectSuitCommandContext(Suit selectedSuit, Player executingPlayer)
        {
            SelectedSuit = selectedSuit;
            ExecutingPlayer = executingPlayer;
        }

        public Suit SelectedSuit { get; }
        public Player ExecutingPlayer { get; }
    }
}
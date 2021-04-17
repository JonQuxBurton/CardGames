namespace SheddingCardGames.Domain
{
    public class SelectSuitCommandContext
    {
        public SelectSuitCommandContext(Suit selectedSuit, Player player)
        {
            SelectedSuit = selectedSuit;
            Player = player;
        }

        public Suit SelectedSuit { get; }
        public Player Player { get; }
    }
}
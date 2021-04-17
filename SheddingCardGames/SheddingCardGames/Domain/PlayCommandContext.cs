namespace SheddingCardGames.Domain
{
    public class PlayCommandContext
    {
        public PlayCommandContext(Player player, Card playedCard)
        {
            Player = player;
            PlayedCard = playedCard;
        }

        public Player Player { get; }
        public Card PlayedCard { get; }
    }
}
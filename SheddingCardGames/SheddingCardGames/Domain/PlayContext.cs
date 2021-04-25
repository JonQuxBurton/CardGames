namespace SheddingCardGames.Domain
{
    public class PlayContext : ICommandContext
    {
        public PlayContext(Player executingPlayer, Card playedCard)
        {
            ExecutingPlayer = executingPlayer;

            PlayedCard = playedCard;
        }

        public Player ExecutingPlayer { get; }
        public Card PlayedCard { get; }
    }
}
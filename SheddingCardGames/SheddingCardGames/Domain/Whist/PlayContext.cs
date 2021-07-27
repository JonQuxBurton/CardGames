namespace SheddingCardGames.Domain.Whist
{
    public class PlayContext : ICommandContext
    {
        public PlayContext(Player executingPlayer, Card cardPlayed)
        {
            ExecutingPlayer = executingPlayer;
            CardPlayed = cardPlayed;
        }

        public Player ExecutingPlayer { get; }
        public Card CardPlayed { get; }
    }
}
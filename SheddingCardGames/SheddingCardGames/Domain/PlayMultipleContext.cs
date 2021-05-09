using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public class PlayMultipleContext : ICommandContext
    {
        public PlayMultipleContext(Player executingPlayer, IImmutableList<Card> cardsPlayed)
        {
            ExecutingPlayer = executingPlayer;
            CardsPlayed = cardsPlayed;
        }

        public Player ExecutingPlayer { get; }
        public IImmutableList<Card> CardsPlayed { get; }
    }
}
using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public class PlayMultipleContext : ICommandContext
    {
        public PlayMultipleContext(Player executingPlayer, IImmutableList<Card> playedCards)
        {
            ExecutingPlayer = executingPlayer;
            PlayedCards = playedCards;
        }

        public Player ExecutingPlayer { get; }
        public IImmutableList<Card> PlayedCards { get; }
    }
}
using System.Collections.Immutable;
using static SheddingCardGames.Domain.CardsUtils;

namespace SheddingCardGames.Domain
{
    public class PlayContext : ICommandContext
    {
        public PlayContext(Player executingPlayer, IImmutableList<Card> cardsPlayed)
        {
            ExecutingPlayer = executingPlayer;
            CardsPlayed = cardsPlayed;
        }
        
        public PlayContext(Player executingPlayer, Card cardPlayed)
        {
            ExecutingPlayer = executingPlayer;
            CardsPlayed = Cards(cardPlayed);
        }

        public Player ExecutingPlayer { get; }
        public IImmutableList<Card> CardsPlayed { get; }
    }
}
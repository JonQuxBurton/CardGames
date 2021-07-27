using System.Collections.Immutable;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;

namespace SheddingCardGames.Tests.Domain.CrazyEights
{
    public class TableCreator
    {
        public static Table Create(StockPile stockPile, DiscardPile discardPile, IImmutableList<Player> players)
        {
            return new Table(stockPile, discardPile, players);
        }
    }
}
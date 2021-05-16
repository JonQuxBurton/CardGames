using System.Collections.Immutable;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class TableCreator
    {
        public static Table Create(StockPile stockPile, DiscardPile discardPile, IImmutableList<Player> players)
        {
            return new Table(stockPile, discardPile, players);
        }
    }
}
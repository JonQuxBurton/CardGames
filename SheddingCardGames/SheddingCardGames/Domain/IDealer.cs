using System.Collections.Generic;

namespace SheddingCardGames.Domain
{
    public interface IDealer
    {
        Board Build(IEnumerable<Player> players);
    }
}
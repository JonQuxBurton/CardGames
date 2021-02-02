using System.Collections.Generic;

namespace SheddingCardGames.Domain
{
    public interface IDealer
    {
        Board Deal(IEnumerable<Player> players);
    }
}
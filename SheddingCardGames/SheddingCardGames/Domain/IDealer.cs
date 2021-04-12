using System.Collections.Generic;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain
{
    public interface IDealer
    {
        Board Deal(IEnumerable<Player> players, CardCollection cardsToDeal, List<DomainEvent> events);
    }
}
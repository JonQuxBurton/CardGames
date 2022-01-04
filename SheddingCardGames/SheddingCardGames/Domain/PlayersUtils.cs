using System.Collections.Generic;
using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public static class PlayersUtils
    {
        public static ImmutableList<Player> Players(params Player[] players)
        {
            return ImmutableList.Create(players);
        }
        
        public static ImmutableList<Player> Players(List<Player> playersList)
        {
            return ImmutableList.Create(playersList.ToArray());
        }
    }
}
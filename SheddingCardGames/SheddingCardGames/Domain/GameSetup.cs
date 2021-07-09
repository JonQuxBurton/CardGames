using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public class GameSetup
    {
        public GameSetup(IImmutableList<Player> players)
        {
            Players = players;
        }

        public IImmutableList<Player> Players { get; }
        public Player PlayerToStart { get; private set; }

        public void WithStartingPlayer(Player playerToStart)
        {
            PlayerToStart = playerToStart;
        }
    }
}
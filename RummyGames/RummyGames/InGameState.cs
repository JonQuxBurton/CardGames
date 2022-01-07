using System;

namespace RummyGames
{
    public class InGameState
    {
        public Guid GameId { get; }
        public Player StartingPlayer { get; }

        public InGameState(Guid gameId, Player startingPlayer)
        {
            GameId = gameId;
            StartingPlayer = startingPlayer;
        }
    }
}
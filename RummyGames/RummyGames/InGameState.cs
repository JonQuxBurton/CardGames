using System;

namespace RummyGames
{
    public class InGameState
    {
        public Guid GameId { get; }
        public Player StartingPlayer { get; }

        private InGameState(Guid gameId, Player startingPlayer)
        {
            GameId = gameId;
            StartingPlayer = startingPlayer;
        }

        public static InGameState Create(Guid gameId, Player startingPlayer)
        {
            return new InGameState(gameId, startingPlayer);
        }
    }
}
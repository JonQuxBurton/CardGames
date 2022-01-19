using System;

namespace RummyGames
{
    public class InGameState
    {
        public InGameState(Guid gameId, Table table, Player startingPlayer)
        {
            GameId = gameId;
            Table = table;
            StartingPlayer = startingPlayer;
        }

        public Guid GameId { get; }
        public Table Table { get; }
        public Player StartingPlayer { get; }
    }
}
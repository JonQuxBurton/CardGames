using System;

namespace RummyGames
{
    public class InGameState
    {
        public InGameState(Guid gameId, Table table, Player startingPlayer, Turn currentTurn)
        {
            GameId = gameId;
            Table = table;
            StartingPlayer = startingPlayer;
            CurrentTurn = currentTurn;
        }

        public Guid GameId { get; }
        public Table Table { get; }
        public Player StartingPlayer { get; }
        public Turn CurrentTurn { get; }
    }
}
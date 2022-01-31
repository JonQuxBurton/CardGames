using System;
using System.Linq;

namespace RummyGames
{
    public class InGameState
    {
        public InGameState(Guid gameId, Table table, Guid startingPlayerId, Turn currentTurn)
        {
            GameId = gameId;
            Table = table;
            StartingPlayerId = startingPlayerId;
            CurrentTurn = currentTurn;
        }

        public Guid GameId { get; }
        public Table Table { get; }
        public Guid StartingPlayerId { get; }
        public Turn CurrentTurn { get; }

        public Player GetPlayer(Guid id) => Table.Players.FirstOrDefault(x => x.Id == id);
    }
}
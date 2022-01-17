using System;
using System.Collections.Generic;
using System.Linq;

namespace RummyGames
{
    public class DataStore
    {
        private readonly List<RummyGame> games = new();
        private readonly List<InGameState> inGameStates = new();

        public RummyGame GetGame(Guid id)
        {
            return games.FirstOrDefault(x => x.Id == id);
        }

        public void AddGame(RummyGame newGame)
        {
            games.Add(newGame);
        }

        public void UpdateGame(RummyGame updatedGame)
        {
            var currentGame = games.FirstOrDefault(x => x.Id == updatedGame.Id);
            games.Remove(currentGame);
            games.Add(updatedGame);
        }

        public InGameState GetInGameState(Guid gameId)
        {
            return inGameStates.FirstOrDefault(x => x.GameId == gameId);
        }

        public void AddInGameState(InGameState newInGameState)
        {
            inGameStates.Add(newInGameState);
        }
    }
}
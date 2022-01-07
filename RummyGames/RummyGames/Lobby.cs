using System;

namespace RummyGames
{
    public class Lobby
    {
        private readonly DataStore dataStore;

        public Lobby(DataStore dataStore)
        {
            this.dataStore = dataStore;
        }

        public RummyGame CreateGame(Player host)
        {
            var newGame = RummyGame.Create(Guid.NewGuid(), host);
            dataStore.AddGame(newGame);
            return newGame;
        }

        public RummyGame JoinGame(RummyGame gameToJoin, Player guest)
        {
            var currentGame = dataStore.GetGame(gameToJoin.Id);
            var updatedGame = RummyGame.WithJoinPlayer(currentGame, guest);
            dataStore.UpdateGame(updatedGame);
            return updatedGame;
        }
    }
}
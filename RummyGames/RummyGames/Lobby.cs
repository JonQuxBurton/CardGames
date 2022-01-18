using System;

namespace RummyGames
{
    public class Lobby
    {
        private readonly DataStore dataStore;
        private readonly IStartingPlayerChooser startingPlayerChooser;

        public Lobby(DataStore dataStore, IStartingPlayerChooser startingPlayerChooser)
        {
            this.dataStore = dataStore;
            this.startingPlayerChooser = startingPlayerChooser;
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

            var startingPlayer = startingPlayerChooser.Choose(updatedGame.Host, updatedGame.Guest);

            dataStore.AddInGameState(InGameState.Create(updatedGame.Id, startingPlayer));

            return updatedGame;
        }
    }
}
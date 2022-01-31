using System;
using CardGamesDomain;

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

            return updatedGame;
        }

        public RummyGame SetupGame(RummyGame gameToSetup)
        {
            var inGameState = SetupInGameState(gameToSetup);
            dataStore.AddInGameState(inGameState);

            return gameToSetup;
        }


        private InGameState SetupInGameState(RummyGame game)
        {
            var startingPlayer = startingPlayerChooser.Choose(game.Host, game.Guest);
            var deckBuilder = new DeckBuilder();

            return new InGameState(game.Id, new Table(new[] {game.Host, game.Guest}, deckBuilder.Build(), null),
                startingPlayer.Id, null);
        }
    }
}
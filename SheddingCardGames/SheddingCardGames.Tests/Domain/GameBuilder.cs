using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Tests.Domain
{
    public class GameBuilder
    {
        private Player[] players;
        private IShuffler shuffler = new DummyShuffler();
        private int startingPlayerNumber = 1;

        public GameBuilder WithPlayers(params Player[] withPlayers)
        {
            players = withPlayers;
            return this;
        }

        public GameBuilder WithShuffler(IShuffler withShuffler)
        {
            shuffler = withShuffler;
            return this;
        }

        public GameBuilder WithStartingPlayer(int withPlayerNumber)
        {
            startingPlayerNumber = withPlayerNumber;
            return this;
        }

        public Game BuildAtStart(CardCollection deck)
        {
            var game = Build();
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());
            game.Deal(new DealContext(deck));
            return game;
        }

        public Game BuildAtReadyToDeal()
        {
            var game = Build();
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());
            return game;
        }

        public Game BuildWithGameState(GameState withGameState)
        {
            var game = Build();
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());
            game.Initialise(withGameState);
            return game;
        }

        public Game Build()
        {
            var randomPlayerChooser = new DummyPlayerChooser(players[startingPlayerNumber - 1]);
            var numberOfPlayers = (NumberOfPlayers) players.Length;

            CrazyEightsRules crazyEightsRules = new BasicVariantRules(numberOfPlayers);
            var game = new Game(
                new Variant(VariantName.OlsenOlsen,
                    new OlsenOlsenVariantCommandFactory(crazyEightsRules, shuffler, randomPlayerChooser)),
                players);

            return game;
        }
    }
}
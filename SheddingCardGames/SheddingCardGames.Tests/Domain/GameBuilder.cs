using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Tests.Domain
{
    public class GameBuilder
    {
        private int numberOfPlayers = 2;
        private readonly SampleData sampleData;
        private IShuffler shuffler = new DummyShuffler();
        private int startingPlayerNumber = 1;

        public GameBuilder()
        {
            sampleData = new SampleData();
        }

        public GameBuilder WithNumberOfPlayers(int withNumberOfPlayers)
        {
            numberOfPlayers = withNumberOfPlayers;
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

        public Game Build()
        {
            var player1 = sampleData.Player1;
            var player2 = sampleData.Player2;

            CrazyEightsRules crazyEightsRules;
            Game game;
            var randomPlayerChooser = new DummyPlayerChooser(sampleData.GetPlayer(startingPlayerNumber));

            if (numberOfPlayers > 2)
            {
                crazyEightsRules = new BasicVariantRules(NumberOfPlayers.Three);
                var player3 = sampleData.Player3;
                game = new Game(
                    new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(crazyEightsRules, shuffler, randomPlayerChooser)),
                    new[] {player1, player2, player3});
            }
            else
            {
                crazyEightsRules = new BasicVariantRules(NumberOfPlayers.Two);
                game = new Game(
                    new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(crazyEightsRules, shuffler, randomPlayerChooser)),
                    new[] {player1, player2});
            }

            return game;
        }
    }
}
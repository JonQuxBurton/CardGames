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

        public Game Build()
        {
            var player1 = sampleData.Player1;
            var player2 = sampleData.Player2;

            IRules rules;
            Game game;

            if (numberOfPlayers > 2)
            {
                rules = new CrazyEightsRules(NumberOfPlayers.Three);
                var player3 = sampleData.Player3;
                game = new Game(
                    new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)),
                    new[] {player1, player2, player3});
            }
            else
            {
                rules = new CrazyEightsRules(NumberOfPlayers.Two);
                game = new Game(
                    new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)),
                    new[] {player1, player2});
            }

            return game;
        }
    }
}
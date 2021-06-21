using System.Linq;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class SpecificGameBuilder
    {
        public Game Build()
        {
            var players = new[] {new Player(1, "Alice"), new Player(2, "Bob")};
            var rules = new BasicVariantRules(CrazyEightsRules.NumberOfPlayers.Two);
            var shuffler = new DummyShuffler();
            var randomPlayerChooser = new DummyPlayerChooser(players.First());

            var variant = new Variant(VariantName.OlsenOlsen,
                new OlsenOlsenVariantCommandFactory(rules, shuffler, randomPlayerChooser));
            var game = new Game(variant, players);

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());

            return game;
        }
    }
}
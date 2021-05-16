using System;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class SpecificGameBuilder
    {
        public Game Build()
        {
            var players = new[] {new Player(1, "Alice"), new Player(2,"Bob")};
            var rules = new CrazyEightsRules(CrazyEightsRules.NumberOfPlayers.Two);
            var shuffler = new DummyShuffler();
            var variant = new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler));
            var game = new Game(variant, players);

            var random = new Random();
            var firstPlayer = players[random.Next(2)];
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(firstPlayer));

            return game;
        }
    }
}
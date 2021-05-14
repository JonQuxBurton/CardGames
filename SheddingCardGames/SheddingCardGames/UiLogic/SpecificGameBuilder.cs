using System;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class SpecificGameBuilder
    {
        public Game Build(CardCollection deck)
        {
            var players = new[] {new Player(1, "Alice"), new Player(2,"Bob")};
            var rules = new Rules(7);
            var shuffler = new DummyShuffler();
            var variant = new Variant(new OlsenOlsenVariantCommandFactory(rules, shuffler));
            var game = new Game(variant, deck, players);

            var random = new Random();
            var firstPlayer = players[random.Next(2)];
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(firstPlayer));

            return game;
        }
    }
}
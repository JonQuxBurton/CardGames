using System;
using SheddingCardGames;
using SheddingCardGames.Domain;

namespace SheddingCardGame.UI
{
    public class SpecificGameBuilder
    {
        public Game Build(CardCollection deck)
        {
            var players = new[] {new Player(1), new Player(2)};
            var rules = new Rules();
            var shuffler = new DummyShuffler();
            var game = new Game(rules, shuffler, players);
            var boardBuilder = new BoardBuilder(rules, shuffler);
            var board = boardBuilder.Build(deck, players);

            var random = new Random();
            var firstPlayer = random.Next(2) + 1;
            game.Setup(board, firstPlayer);

            return game;
        }
    }
}
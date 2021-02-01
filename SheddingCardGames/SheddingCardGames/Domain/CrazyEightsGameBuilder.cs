using System;
using System.Diagnostics.CodeAnalysis;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class CrazyEightsGameBuilder
    {
        public Game Build(CardCollection deck)
        {
            var players = new[] { new Player(1), new Player(2) };
            var rules = new Rules(7);
            var shuffler = new Shuffler();
            var boardBuilder = new Dealer(rules, shuffler, deck);
            var game = new Game(rules, shuffler, boardBuilder, players);

            var random = new Random();
            var firstPlayer = random.Next(2) + 1;
            game.ChooseStartingPlayer(firstPlayer);

            return game;
        }
    }
}
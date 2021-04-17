using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class CrazyEightsGameBuilder
    {
        public Game Build(CardCollection deck, int numberOfPlayers)
        {
            var players = new List<Player> { new Player(1, "Alice"), new Player(2, "Bob") };
            var rules = new Rules(7);
            var random = new Random();

            if (numberOfPlayers > 2)
            {
                players.Add(new Player(3, "Carol"));
                rules = new Rules(5);
            }

            var shuffler = new Shuffler();
            var dealer = new Dealer(rules);
            var game = new Game(rules, shuffler, dealer, deck, players.ToArray());
            var firstPlayer = players[random.Next(numberOfPlayers)];

            game.ChooseStartingPlayer(firstPlayer);

            return game;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class CrazyEightsGameBuilder : ICrazyEightsGameBuilder
    {
        public Game Build(VariantName variantNameName, CardCollection deck, int numberOfPlayers)
        {
            var players = new List<Player> {new Player(1, "Alice"), new Player(2, "Bob")};
            var rules = new Rules(7);
            var random = new Random();

            if (numberOfPlayers > 2)
            {
                players.Add(new Player(3, "Carol"));
                rules = new Rules(5);
            }

            var shuffler = new Shuffler();
            
            Variant variant;
            if (variantNameName == VariantName.OlsenOlsen)
                variant = new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler));
            else
                variant = new Variant(VariantName.Basic, new BasicVariantCommandFactory(rules, shuffler));

            var game = new Game(variant, deck, players.ToArray());
            var firstPlayer = players[random.Next(numberOfPlayers)];

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(firstPlayer));

            return game;
        }
    }
}
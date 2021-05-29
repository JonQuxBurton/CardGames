using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class CrazyEightsGameBuilder : ICrazyEightsGameBuilder
    {
        public Game Build(VariantName variantNameName, int numberOfPlayers)
        {
            var players = new List<Player> {new Player(1, "Alice"), new Player(2, "Bob")};
            var rules = new BasicVariantRules(NumberOfPlayers.Two);
            var random = new Random();

            if (numberOfPlayers > 2)
            {
                players.Add(new Player(3, "Carol"));
                rules = new BasicVariantRules(NumberOfPlayers.Two);
            }
            
            if (numberOfPlayers > 3)
            {
                players.Add(new Player(4, "Dan"));
                rules = new BasicVariantRules(NumberOfPlayers.Four);
            }

            var shuffler = new Shuffler();
            
            Variant variant;
            if (variantNameName == VariantName.OlsenOlsen)
                variant = new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler));
            else
                variant = new Variant(VariantName.Basic, new BasicVariantCommandFactory(rules, shuffler));

            var game = new Game(variant, players.ToArray());
            var firstPlayer = players[random.Next(numberOfPlayers)];

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(firstPlayer));

            return game;
        }
    }
}
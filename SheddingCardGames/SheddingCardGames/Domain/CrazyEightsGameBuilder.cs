using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class CrazyEightsGameBuilder : ICrazyEightsGameBuilder
    {
        public Game Build(IShuffler shuffler, VariantName variantNameName, int numberOfPlayers)
        {
            var numberOfPlayersValue = NumberOfPlayers.Two;
            var players = new List<Player> {new Player(1, "Alice"), new Player(2, "Bob")};

            if (numberOfPlayers == 3)
            {
                numberOfPlayersValue = NumberOfPlayers.Three;
                players.Add(new Player(3, "Carol"));
            }
            else if (numberOfPlayers == 4)
            {
                numberOfPlayersValue = NumberOfPlayers.Four;
                players.Add(new Player(4, "Dan"));
            }

            CrazyEightsRules rules;

            Variant variant;
            if (variantNameName == VariantName.OlsenOlsen)
            {
                rules = new OlsenOlsenVariantRules(numberOfPlayersValue);
                variant = new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler));
            }
            else
            {
                rules = new BasicVariantRules(numberOfPlayersValue);
                variant = new Variant(VariantName.Basic, new BasicVariantCommandFactory(rules, shuffler));
            }

            var game = new Game(variant, players.ToArray());

            return game;
        }
    }
}
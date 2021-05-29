using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.BasicVariantRules;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class TestCrazyEightsGameBuilder : ICrazyEightsGameBuilder

    {
        private readonly int firstPlayerNumber;

        public TestCrazyEightsGameBuilder(int firstPlayerNumber)
        {
            this.firstPlayerNumber = firstPlayerNumber;
        }

        public Game Build(VariantName variantName, int numberOfPlayers)
        {
            var numberOfPlayers2 = NumberOfPlayers.Two;
            var players = new List<Player> { new Player(1, "Alice"), new Player(2, "Bob") };

            if (numberOfPlayers == 3)
            {
                numberOfPlayers2 = NumberOfPlayers.Three;
                players.Add(new Player(3, "Carol"));
            }
            else if (numberOfPlayers == 4)
            {
                numberOfPlayers2 = NumberOfPlayers.Four;
                players.Add(new Player(4, "Dan"));
            }
            
            CrazyEightsRules rules;

            var shuffler = new DummyShuffler();

            Variant variant;
            if (variantName == VariantName.OlsenOlsen)
            {
                rules = new OlsenOlsenVariantRules(numberOfPlayers2);
                variant = new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler));
            }
            else
            {
                rules = new BasicVariantRules(numberOfPlayers2);
                variant = new Variant(VariantName.Basic, new BasicVariantCommandFactory(rules, shuffler));
            }

            var game = new Game(variant, players.ToArray());
            var firstPlayer = players[firstPlayerNumber-1];

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(firstPlayer));

            return game;
        }
    }
}
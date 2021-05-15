using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Domain
{
    [ExcludeFromCodeCoverage]
    public class TestCrazyEightsGameBuilder : ICrazyEightsGameBuilder

    {
        private readonly int firstPlayerNumber;
        private readonly int handSize;

        public TestCrazyEightsGameBuilder(int handSize, int firstPlayerNumber)
        {
            this.handSize = handSize;
            this.firstPlayerNumber = firstPlayerNumber;
        }

        public Game Build(VariantName variantName, CardCollection deck, int numberOfPlayers)
        {
            var players = new List<Player> {new Player(1, "Alice"), new Player(2, "Bob")};
            var rules = new CrazyEightsRules(NumberOfPlayers.Two);

            if (numberOfPlayers > 2)
            {
                players.Add(new Player(3, "Carol"));
                rules = new CrazyEightsRules(NumberOfPlayers.Three);
            }
            
            if (numberOfPlayers > 3)
            {
                players.Add(new Player(4, "Dan"));
                rules = new CrazyEightsRules(NumberOfPlayers.Four);
            }

            var shuffler = new DummyShuffler();

            Variant variant;
            if (variantName == VariantName.OlsenOlsen)
                variant = new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler));
            else
                variant = new Variant(VariantName.Basic, new BasicVariantCommandFactory(rules, shuffler));

            var game = new Game(variant, deck, players.ToArray());
            var firstPlayer = players[firstPlayerNumber-1];

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(firstPlayer));

            return game;
        }
    }
}
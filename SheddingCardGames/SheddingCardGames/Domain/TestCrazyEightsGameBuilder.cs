using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SheddingCardGames.UiLogic;

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

        public Game Build(CardCollection deck, int numberOfPlayers)
        {
            var players = new List<Player> {new Player(1, "Alice"), new Player(2, "Bob")};
            var rules = new Rules(handSize);

            if (numberOfPlayers > 2) players.Add(new Player(3, "Carol"));

            var shuffler = new DummyShuffler();
            var game = new Game(rules, shuffler, deck, players.ToArray());
            var firstPlayer = players[firstPlayerNumber-1];

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(firstPlayer));

            return game;
        }
    }
}
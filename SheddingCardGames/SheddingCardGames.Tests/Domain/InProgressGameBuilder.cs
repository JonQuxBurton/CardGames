using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Tests.Domain
{
    public class InProgressGameBuilder
    {
        private IShuffler shuffler = new DummyShuffler();
        private readonly List<Player> players = new List<Player>();
        private CurrentTurn currentTurn;
        private DiscardPile discardPile = new DiscardPile();
        private int startingPlayer = 1;
        private StockPile stockPile = new StockPile(new CardCollection());

        public InProgressGameBuilder WithShuffler(IShuffler withShuffler)
        {
            shuffler = withShuffler;
            return this;
        }

        public InProgressGameBuilder WithStartingPlayer(int playerNumber)
        {
            startingPlayer = playerNumber;
            return this;
        }

        public InProgressGameBuilder WithPlayer(Player player)
        {
            players.Add(player);
            return this;
        }

        public InProgressGameBuilder WithStockPile(StockPile withStockPile)
        {
            stockPile = withStockPile;
            return this;
        }

        public InProgressGameBuilder WithDiscardPile(DiscardPile withDiscardPile)
        {
            discardPile = withDiscardPile;
            return this;
        }

        public InProgressGameBuilder WithCurrentTurn(CurrentTurn withCurrentTurn)
        {
            currentTurn = withCurrentTurn;
            return this;
        }

        public Game Build()
        {
            if (!players.Any())
            {
                var sampleData = new SampleData();
                players.Add(sampleData.Player1);
                players.Add(sampleData.Player2);
            }

            var deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards),
                players.Select(x => x.Hand).ToArray()).Build();
            var rules = new Rules(7);
            var sut = new Game(rules, shuffler, new Dealer(rules), deck, new[]
            {
                players.ElementAt(0),
                players.ElementAt(1)
            });
            var expectedBoard = new Table(stockPile, discardPile, players.ToArray());
            var gameState = new GameState(GamePhase.InGame, startingPlayer, expectedBoard, currentTurn);

            sut.Initialise(gameState);

            return sut;
        }
    }
}
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Tests.Domain
{
    public class InProgressGameBuilder
    {
        private DiscardPile discardPile = new DiscardPile();
        private Player player1 = new Player(1);
        private Player player2 = new Player(2);
        private int startingPlayer = 1;
        private StockPile stockPile = new StockPile(new CardCollection());
        private Turn currentTurn;

        public InProgressGameBuilder WithStartingPlayer(int playerNumber)
        {
            startingPlayer = playerNumber;
            return this;
        }

        public InProgressGameBuilder WithPlayer1Hand(CardCollection hand)
        {
            player1 = new Player(1) {Hand = hand};
            return this;
        }

        public InProgressGameBuilder WithPlayer2Hand(CardCollection hand)
        {
            player2 = new Player(2) {Hand = hand};
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

        public InProgressGameBuilder WithCurrentTurn(Turn withCurrentTurn)
        {
            currentTurn = withCurrentTurn;
            return this;
        }


        public Game Build()
        {
            var rules = new Rules(7);
            var sut = new Game(rules, new DummyShuffler(), new Dealer(rules, new DummyShuffler(), new CardCollection()),
                new[] {new Player(1), new Player(2)});
            var expectedBoard = new Board(player1, player2, stockPile, discardPile);
            var gameState = new GameState(GamePhase.InGame, startingPlayer, expectedBoard, currentTurn);

            sut.Initialise(gameState);

            return sut;
        }
    }
}
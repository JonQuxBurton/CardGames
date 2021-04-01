using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Tests.Domain
{
    public class InProgressGameBuilder
    {
        private DiscardPile discardPile = new DiscardPile();
        private Player player1;
        private Player player2;
        private int startingPlayer = 1;
        private StockPile stockPile = new StockPile(new CardCollection());
        private Turn currentTurn;

        public InProgressGameBuilder()
        {
            var sampleData = new SampleData();
            player1 = sampleData.Player1;
            player2 = sampleData.Player2;
        }

        public InProgressGameBuilder WithStartingPlayer(int playerNumber)
        {
            startingPlayer = playerNumber;
            return this;
        }

        public InProgressGameBuilder WithPlayer1(Player player)
        {
            player1 = player;
            return this;
        }

        //public InProgressGameBuilder WithPlayer1Hand(CardCollection hand)
        //{
        //    player1.Hand = hand;
        //    return this;
        //}

        public InProgressGameBuilder WithPlayer2(Player player)
        {
            player2 = player;
            return this;
        }
        
        //public InProgressGameBuilder WithPlayer2Hand(CardCollection hand)
        //{
        //    player2.Hand = hand;
        //    return this;
        //}

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
            var deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1.Hand, player2.Hand).Build();
            var rules = new Rules(7);
            var sut = new Game(rules, new DummyShuffler(), new Dealer(rules),
                new[] {player1, player2}, deck);
            var expectedBoard = new Board(stockPile, discardPile, player1, player2);
            var gameState = new GameState(GamePhase.InGame, startingPlayer, expectedBoard, currentTurn);

            sut.Initialise(gameState);

            return sut;
        }
    }
}
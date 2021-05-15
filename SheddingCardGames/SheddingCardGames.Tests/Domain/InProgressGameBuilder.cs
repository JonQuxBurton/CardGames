using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Tests.Domain
{
    public class InProgressGameBuilder
    {
        private Player currentPlayer;
        private CurrentTurn currentTurn;
        private DiscardPile discardPile = new DiscardPile();
        private IShuffler shuffler = new DummyShuffler();
        private StockPile stockPile = new StockPile(new CardCollection());
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection player3Hand = new CardCollection();
        private int numberOfPlayers = 2;

        public InProgressGameBuilder WithShuffler(IShuffler withShuffler)
        {
            shuffler = withShuffler;
            return this;
        }

        public InProgressGameBuilder WithCurrentPlayer(Player withCurrentPlayer)
        {
            currentPlayer = withCurrentPlayer;
            return this;
        }

        public InProgressGameBuilder WithPlayer1Hand(CardCollection hand)
        {
            player1Hand = hand;
            return this;
        }

        public InProgressGameBuilder WithPlayer2Hand(CardCollection hand)
        {
            player2Hand = hand;
            return this;
        }

        public InProgressGameBuilder WithPlayer3Hand(CardCollection hand)
        {
            numberOfPlayers = 3;
            player3Hand = hand;
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

        public InProgressGameBuilder WithDiscardCard(Card withDiscardCard)
        {
            discardPile = new DiscardPile(Cards(withDiscardCard));
            discardPile.TurnUpTopCard();
            return this;
        }

        public InProgressGameBuilder WithCurrentTurn(CurrentTurn withCurrentTurn)
        {
            currentTurn = withCurrentTurn;
            return this;
        }

        public Game Build()
        {
            CardCollection deck;
            Game game;
            Table expectedTable;

            var sampleData = new SampleData();
            var player1 = sampleData.Player1;
            player1.Hand = player1Hand;
            var player2 = sampleData.Player2;
            player2.Hand = player2Hand;

            if (numberOfPlayers > 2)
            {
                var rules = new CrazyEightsRules(NumberOfPlayers.Three);
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand, player2Hand, player3Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2, player3 });
                expectedTable = new Table(stockPile, discardPile, player1, player2, player3);
            }
            else
            {
                var rules = new CrazyEightsRules(NumberOfPlayers.Two);
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand, player2Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2 });
                expectedTable = new Table(stockPile, discardPile, player1, player2);
            }

            var gameState = new GameState
            {
                CurrentTable = expectedTable,
                PlayerToStart = currentPlayer,
                CurrentTurn = currentTurn
            };
            game.Initialise(gameState);

            return game;
        }
    }
}
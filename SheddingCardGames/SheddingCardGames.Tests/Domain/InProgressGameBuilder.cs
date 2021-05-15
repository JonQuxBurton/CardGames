using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Tests.Domain
{
    public class InProgressGameBuilder
    {
        private readonly List<Player> players = new List<Player>();
        private Player currentPlayer;
        private CurrentTurn currentTurn;
        private DiscardPile discardPile = new DiscardPile();
        private IShuffler shuffler = new DummyShuffler();
        private StockPile stockPile = new StockPile(new CardCollection());
        private NumberOfPlayers numberOfPlayers = NumberOfPlayers.Two;

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
            if (!players.Any())
            {
                var sampleData = new SampleData();
                players.Add(sampleData.Player1);
                players.Add(sampleData.Player2);
            }

            if (players.Count == 3)
                numberOfPlayers = NumberOfPlayers.Three;
            else if (players.Count == 4)
                numberOfPlayers = NumberOfPlayers.Four;

            var deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards),
                players.Select(x => x.Hand).ToArray()).Build();
            var rules = new CrazyEightsRules(numberOfPlayers);
            var sut = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, players.ToArray());
            var expectedTable = new Table(stockPile, discardPile, players.ToArray());

            var gameState = new GameState
            {
                CurrentTable = expectedTable,
                PlayerToStart = currentPlayer,
                CurrentTurn = currentTurn
            };
            sut.Initialise(gameState);

            return sut;
        }
    }
}
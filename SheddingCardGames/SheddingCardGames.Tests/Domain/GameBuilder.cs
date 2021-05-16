using System.Collections.Immutable;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules;
using static SheddingCardGames.Domain.PlayersUtils;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.Domain
{
    public class GameBuilder
    {
        private Player currentPlayer;
        private CurrentTurn currentTurn;

        private IShuffler shuffler = new DummyShuffler();
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection player3Hand = new CardCollection();
        private StockPile stockPile = new StockPile(new CardCollection());
        private int startingPlayerNumber = 1;
        private int numberOfPlayers = 2;
        private SampleData sampleData;
        private DiscardPile discardPile = new DiscardPile(Cards(Card(1, Clubs)));

        public GameBuilder WithCurrentPlayer(Player withCurrentPlayer)
        {
            currentPlayer = withCurrentPlayer;
            return this;
        }

        public GameBuilder WithCurrentTurn(CurrentTurn withCurrentTurn)
        {
            currentTurn = withCurrentTurn;
            return this;
        }

        public GameBuilder WithPlayer1Hand(CardCollection hand)
        {
            player1Hand = hand;
            return this;
        }

        public GameBuilder WithPlayer2Hand(CardCollection hand)
        {
            player2Hand = hand;
            return this;
        }
        public GameBuilder WithPlayer3Hand(CardCollection hand)
        {
            numberOfPlayers = 3;
            player3Hand = hand;
            return this;
        }

        public GameBuilder WithDiscardCard(Card card)
        {
            discardPile = new DiscardPile(Cards(card));
            discardPile.TurnUpTopCard();
            return this;
        }

        public GameBuilder WithDiscardPile(DiscardPile withDiscardPile)
        {
            discardPile = withDiscardPile;
            return this;
        }

        public GameBuilder WithStockPile(StockPile withStockPile)
        {
            stockPile = withStockPile;
            return this;
        }

        public GameBuilder WithStockPile(CardCollection withStockPile)
        {
            stockPile = new StockPile(withStockPile);
            return this;
        }

        public GameBuilder WithStartingPlayer(int withPlayerNumber)
        {
            startingPlayerNumber = withPlayerNumber;
            return this;
        }

        public GameBuilder WithShuffler(IShuffler withShuffler)
        {
            shuffler = withShuffler;
            return this;
        }

        private Game BuildGame()
        {
            var player1 = sampleData.Player1;
            var player2 = sampleData.Player2;

            IRules rules;
            CardCollection deck;
            Game game;

            if (numberOfPlayers > 2)
            {
                rules = new CrazyEightsRules(NumberOfPlayers.Three);
                var player3 = sampleData.Player3;
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand, player2Hand, player3Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), new[] { player1, player2, player3 });
            }
            else
            {
                rules = new CrazyEightsRules(NumberOfPlayers.Two);
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand, player2Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), new[] { player1, player2 });
            }

            return game;
        }

        public Game BuildInProgressGame()
        {
            sampleData = new SampleData();

            Table expectedTable;
            IImmutableList<Player> players;
            if (numberOfPlayers > 2)
            {
                players = Players(sampleData.Player1, sampleData.Player2, sampleData.Player3);
                expectedTable = new Table(stockPile, discardPile, Players(sampleData.Player1, sampleData.Player2, sampleData.Player3));
            }
            else
            {
                players = Players(sampleData.Player1, sampleData.Player2);
                expectedTable = new Table(stockPile, discardPile, Players(sampleData.Player1, sampleData.Player2));
            }
            var player1 = sampleData.Player1;
            player1.Hand = player1Hand;
            var player2 = sampleData.Player2;
            player2.Hand = player2Hand;

            if (numberOfPlayers > 2)
            {
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;
            }

            var game = BuildGame();

            var gameState = new GameState(players)
            {
                CurrentTable = expectedTable,
                PlayerToStart = currentPlayer,
                CurrentTurn = currentTurn
            };
            game.Initialise(gameState);

            return game;

        }

        public Game BuildReadyToDealGame()
        {
            sampleData = new SampleData();
            var player1 = sampleData.Player1;
            player1.Hand = player1Hand;
            var player2 = sampleData.Player2;
            player2.Hand = player2Hand;

            if (numberOfPlayers > 2)
            {
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;
            }

            var game = BuildGame();
            
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(sampleData.GetPlayer(startingPlayerNumber)));

            return game;
        }

        public Game BuildAtStartGame()
        {
            sampleData = new SampleData();
            CardCollection deck;

            if (numberOfPlayers > 2)
            {
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand, player2Hand, player3Hand).Build();
            }
            else
            {
            
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand, player2Hand).Build();
            }


            var game = BuildGame();

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(sampleData.GetPlayer(startingPlayerNumber)));
            game.Deal(new DealContext(deck));

            return game;
        }
    }
}
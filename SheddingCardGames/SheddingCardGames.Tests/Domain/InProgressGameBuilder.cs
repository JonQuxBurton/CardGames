using System.Collections.Immutable;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class InProgressGameBuilder
    {
        private readonly Game game;

        private Player currentPlayer;
        private StateOfTurn currentTurn;
        private DiscardPile discardPile = new DiscardPile(CardsUtils.Cards(CardsUtils.Card(1, Suit.Clubs)));
        private int numberOfPlayers = 2;
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection player3Hand = new CardCollection();
        private StockPile stockPile = new StockPile(new CardCollection());

        public InProgressGameBuilder(Game game)
        {
            this.game = game;
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

        public InProgressGameBuilder WithDiscardCard(Card card)
        {
            discardPile = new DiscardPile(CardsUtils.Cards(card));
            discardPile.TurnUpTopCard();
            return this;
        }

        public InProgressGameBuilder WithDiscardPile(DiscardPile withDiscardPile)
        {
            discardPile = withDiscardPile;
            return this;
        }

        public InProgressGameBuilder WithStockPile(StockPile withStockPile)
        {
            stockPile = withStockPile;
            return this;
        }

        public InProgressGameBuilder WithCurrentPlayer(Player withCurrentPlayer)
        {
            currentPlayer = withCurrentPlayer;
            return this;
        }

        public InProgressGameBuilder WithCurrentTurn(StateOfTurn withCurrentTurn)
        {
            currentTurn = withCurrentTurn;
            return this;
        }

        public Game Build()
        {
            var player1 = game.GetPlayer(1);
            var player2 = game.GetPlayer(2);
            player1.Hand = player1Hand;
            player2.Hand = player2Hand;

            if (numberOfPlayers > 2)
                game.GetPlayer(3).Hand = player3Hand;

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());

            Table expectedTable;
            IImmutableList<Player> players;
            if (numberOfPlayers > 2)
            {
                var player3 = game.GetPlayer(3);
                players = PlayersUtils.Players(player1, player2, player3);
                expectedTable = new Table(stockPile, discardPile, PlayersUtils.Players(player1, player2, player3));
            }
            else
            {
                players = PlayersUtils.Players(player1, player2);
                expectedTable = new Table(stockPile, discardPile, PlayersUtils.Players(player1, player2));
            }

            var gameState = new GameState()
            {
                GameSetup = new GameSetup(players),
                CurrentTable = expectedTable,
                CurrentStateOfTurn = currentTurn,
                CurrentStateOfPlay = game.GameState.CurrentStateOfPlay
            };
            gameState.GameSetup.WithStartingPlayer(currentPlayer);
            game.Initialise(gameState);

            return game;
        }
    }
}
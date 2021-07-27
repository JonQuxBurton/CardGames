using System.Collections.Generic;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;

namespace SheddingCardGames.Tests.Domain.CrazyEights
{
    public class GameStateBuilder
    {
        private Player currentPlayer;
        private StateOfPlay currentStateOfPlay;
        private StateOfTurn currentTurn;
        private DiscardPile discardPile;
        private StockPile stockPile;
        private readonly List<Player> players = new List<Player>();

        public GameStateBuilder WithPlayer1(Player withPlayer1, CardCollection withPlayer1Hand)
        {
            withPlayer1.Hand = withPlayer1Hand;
            players.Add(withPlayer1);
            return this;
        }
        
        public GameStateBuilder WithPlayer2(Player withPlayer2, CardCollection withPlayer2Hand)
        {
            withPlayer2.Hand = withPlayer2Hand;
            players.Add(withPlayer2);
            return this;
        }
        
        public GameStateBuilder WithPlayer3(Player withPlayer3, CardCollection withPlayer3Hand)
        {
            withPlayer3.Hand = withPlayer3Hand;
            players.Add(withPlayer3);
            return this;
        }

        public GameStateBuilder WithDiscardCard(Card card)
        {
            discardPile = new DiscardPile(CardsUtils.Cards(card));
            discardPile.TurnUpTopCard();
            return this;
        }

        public GameStateBuilder WithDiscardPile(DiscardPile withDiscardPile)
        {
            discardPile = withDiscardPile;
            return this;
        }

        public GameStateBuilder WithStockPile(StockPile withStockPile)
        {
            stockPile = withStockPile;
            return this;
        }

        public GameStateBuilder WithCurrentPlayer(Player withCurrentPlayer)
        {
            currentPlayer = withCurrentPlayer;
            return this;
        }

        public GameStateBuilder WithCurrentTurn(StateOfTurn withCurrentTurn)
        {
            currentTurn = withCurrentTurn;
            return this;
        }
        
        public GameStateBuilder WithCurrentStateOfPlay(StateOfPlay withCurrentStateOfPlay)
        {
            currentStateOfPlay = withCurrentStateOfPlay;
            return this;
        }

        public GameState Build()
        {
            var playersList = PlayersUtils.Players(players.ToArray());
            var expectedTable = new Table(stockPile, discardPile, playersList);

            var gameState = new GameState
            {
                GameSetup = new GameSetup(playersList),
                CurrentTable = expectedTable,
                CurrentStateOfTurn = currentTurn,
                CurrentStateOfPlay = currentStateOfPlay
            };
            gameState.GameSetup.WithStartingPlayer(currentPlayer);

            return gameState;
        }
    }
}
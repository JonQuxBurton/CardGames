using System.Linq;
using CardGamesDomain;

namespace RummyGames
{
    public class InGameController
    {
        private readonly IShuffler shuffler;
        private readonly int numberOfCardsToDeal;

        public InGameController(IShuffler shuffler)
        {
            this.shuffler = shuffler;
            numberOfCardsToDeal = 10;
        }

        public InGameState ShuffleDeck(InGameState inGameState)
        {
            var shuffledDeck = new Deck(shuffler.Shuffle(inGameState.Table.Deck.Cards));

            return new InGameState(inGameState.GameId, new Table(inGameState.Table.Players, shuffledDeck, null),
                inGameState.StartingPlayer, null);
        }

        public InGameState Deal(InGameState inGameState)
        {
            var currentTable = inGameState.Table;
            var cards = currentTable.Deck.Cards.ToArray();

            Player newPlayer1 = currentTable.Players.ElementAt(0);
            Player newPlayer2 = currentTable.Players.ElementAt(1);
            var deckCounter = 0;
            
            Card nextCard;

            for (var i = 0; i < numberOfCardsToDeal; i++)
            {
                nextCard = cards.ElementAt(deckCounter++);
                newPlayer1 = MoveCardToPlayer(newPlayer1, nextCard);

                nextCard = cards.ElementAt(deckCounter++);
                newPlayer2 = MoveCardToPlayer(newPlayer2, nextCard);
            }

            nextCard = cards.ElementAt(deckCounter++);
            var discardPile = new DiscardPile(nextCard);
            var stockPile = new StockPile(cards.Skip(deckCounter));
            var newTable = new Table(new []{newPlayer1, newPlayer2}, currentTable.Deck, discardPile, stockPile);

            return new InGameState(inGameState.GameId, newTable, inGameState.StartingPlayer, new Turn(1, inGameState.StartingPlayer));
        }

        public Result TakeFromStockPile(InGameState currentGameState, Player playerToTake)
        {
            if (playerToTake.Id != currentGameState.CurrentTurn.CurrentPlayer.Id)
                return new Result(false, ErrorKey.NotTurn, currentGameState);

            if (currentGameState.CurrentTurn.TakenCard != null)
                return new Result(false, ErrorKey.AlreadyTaken, currentGameState);

            var currentTable = currentGameState.Table;
            Player newPlayer1;
            Player newPlayer2;
            Card takenCard = currentGameState.Table.StockPile.TopCard;

            if (playerToTake.Id == currentGameState.Table.Players.First().Id)
            {
                newPlayer1 = new Player(playerToTake.Id, playerToTake.Name, new Hand(
                    playerToTake.Hand.Cards.Append(takenCard)));
                newPlayer2 = currentTable.Players.ElementAt(1);
            }
            else
            {
                newPlayer1 = currentTable.Players.ElementAt(0);
                newPlayer2 = new Player(playerToTake.Id, playerToTake.Name, new Hand(
                    playerToTake.Hand.Cards.Append(takenCard)));
            }


            var newStockPile = new StockPile(currentGameState.Table.StockPile.CardsWithoutTopCard);

            var newTable = new Table(new[] { newPlayer1, newPlayer2 }, currentTable.Deck, currentGameState.Table.DiscardPile, newStockPile);

            var newTurn = new Turn(currentGameState.CurrentTurn.Number, currentGameState.CurrentTurn.CurrentPlayer, takenCard);

            return new Result(true, ErrorKey.None, new InGameState(currentGameState.GameId, newTable, currentGameState.StartingPlayer, newTurn));
        }

        public Result TakeFromDiscardPile(InGameState currentGameState, Player playerToTake)
        {
            if (playerToTake.Id != currentGameState.CurrentTurn.CurrentPlayer.Id)
                return new Result(false, ErrorKey.NotTurn, currentGameState);

            if (currentGameState.CurrentTurn.TakenCard != null)
                return new Result(false, ErrorKey.AlreadyTaken, currentGameState);

            Card takenCard = currentGameState.Table.DiscardPile.TurnedUpCard;

            var currentTable = currentGameState.Table;
            Player newPlayer1;
            Player newPlayer2;

            if (playerToTake.Id == currentGameState.Table.Players.First().Id)
            {
                newPlayer1 = new Player(playerToTake.Id, playerToTake.Name, new Hand(
                    playerToTake.Hand.Cards.Append(takenCard)));
                newPlayer2 = currentTable.Players.ElementAt(1);
            }
            else
            {
                newPlayer1 = currentTable.Players.ElementAt(0);
                newPlayer2 = new Player(playerToTake.Id, playerToTake.Name, new Hand(
                    playerToTake.Hand.Cards.Append(takenCard)));
            }

            var newDiscardPile = new DiscardPile(currentGameState.Table.DiscardPile.RestOfCards);

            var newTable = new Table(new[] { newPlayer1, newPlayer2 }, currentTable.Deck, newDiscardPile, currentGameState.Table.StockPile);

            var newTurn = new Turn(currentGameState.CurrentTurn.Number, currentGameState.CurrentTurn.CurrentPlayer, takenCard);

            return new Result(true, ErrorKey.None, new InGameState(currentGameState.GameId, newTable, currentGameState.StartingPlayer, newTurn));
        }

        private Player GetNextPlayer(InGameState inGameState)
        {
            if (inGameState.CurrentTurn.CurrentPlayer == inGameState.Table.Players.ElementAt(1))
                return inGameState.Table.Players.ElementAt(0);

            return inGameState.Table.Players.ElementAt(1);
        }

        private static Player MoveCardToPlayer(Player currentPlayer, Card cardToDeal)
        {
            var newPlayer = new Player(currentPlayer.Id, currentPlayer.Name,
                new Hand(currentPlayer.Hand.Cards.Append(cardToDeal)));
            return newPlayer;
        }
    }
}
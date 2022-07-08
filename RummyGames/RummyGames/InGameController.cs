using System.Collections.Generic;
using System.Linq;
using CardGamesDomain;

namespace RummyGames
{
    public class InGameController
    {
        private readonly int numberOfCardsToDeal;
        private readonly IShuffler shuffler;

        public InGameController(IShuffler shuffler)
        {
            this.shuffler = shuffler;
            numberOfCardsToDeal = 10;
        }

        public InGameState ShuffleDeck(InGameState inGameState)
        {
            var shuffledDeck = new Deck(shuffler.Shuffle(inGameState.Table.Deck.Cards));

            return new InGameState(inGameState.GameId, new Table(inGameState.Table.Players, shuffledDeck, null),
                inGameState.StartingPlayerId, null);
        }

        public InGameState Deal(InGameState inGameState)
        {
            var currentTable = inGameState.Table;
            var cards = currentTable.Deck.Cards.ToArray();

            var newPlayer1 = currentTable.Players.ElementAt(0);
            var newPlayer2 = currentTable.Players.ElementAt(1);
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
            var newTable = new Table(new[] {newPlayer1, newPlayer2}, currentTable.Deck, discardPile, stockPile);

            return new InGameState(inGameState.GameId, newTable, inGameState.StartingPlayerId,
                new Turn(1, inGameState.StartingPlayerId));
        }

        public Result TakeFromStockPile(InGameState currentGameState, Player playerToTake)
        {
            if (playerToTake.Id != currentGameState.CurrentTurn.CurrentPlayerId)
                return new Result(false, ErrorKey.NotTurn, currentGameState);

            if (currentGameState.CurrentTurn.HasTakenCard())
                return new Result(false, ErrorKey.AlreadyTaken, currentGameState);

            var currentTable = currentGameState.Table;
            Player newPlayer1;
            Player newPlayer2;
            var takenCard = currentGameState.Table.StockPile.TopCard;

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

            var newTable = new Table(new[] {newPlayer1, newPlayer2}, currentTable.Deck,
                currentGameState.Table.DiscardPile, newStockPile);

            var newTurn = new Turn(currentGameState.CurrentTurn.Number, currentGameState.CurrentTurn.CurrentPlayerId,
                takenCard);

            return new Result(true, ErrorKey.None,
                new InGameState(currentGameState.GameId, newTable, currentGameState.StartingPlayerId, newTurn));
        }

        public Result TakeFromDiscardPile(InGameState currentGameState, Player playerToTake)
        {
            if (playerToTake.Id != currentGameState.CurrentTurn.CurrentPlayerId)
                return new Result(false, ErrorKey.NotTurn, currentGameState);

            if (currentGameState.CurrentTurn.HasTakenCard())
                return new Result(false, ErrorKey.AlreadyTaken, currentGameState);

            var takenCard = currentGameState.Table.DiscardPile.TurnedUpCard;

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

            var newTable = new Table(new[] {newPlayer1, newPlayer2}, currentTable.Deck, newDiscardPile,
                currentGameState.Table.StockPile);

            var newTurn = new Turn(currentGameState.CurrentTurn.Number, currentGameState.CurrentTurn.CurrentPlayerId,
                null, takenCard);

            return new Result(true, ErrorKey.None,
                new InGameState(currentGameState.GameId, newTable, currentGameState.StartingPlayerId, newTurn));
        }

        public Result Discard(InGameState currentGameState, Player playerToDiscard, Card cardToDiscard)
        {
            if (playerToDiscard.Id != currentGameState.CurrentTurn.CurrentPlayerId)
                return new Result(false, ErrorKey.NotTurn, currentGameState);

            if (!currentGameState.CurrentTurn.HasTakenCard())
                return new Result(false, ErrorKey.InvalidAction, currentGameState);

            if (currentGameState.CurrentTurn.CardTakenFromDiscardPile != null &&
                currentGameState.CurrentTurn.CardTakenFromDiscardPile.Equals(cardToDiscard))
                return new Result(false, ErrorKey.InvalidAction, currentGameState);

            var currentTable = currentGameState.Table;
            Player newPlayer1;
            Player newPlayer2;

            if (playerToDiscard.Id == currentGameState.Table.Players.First().Id)
            {
                newPlayer1 = new Player(playerToDiscard.Id, playerToDiscard.Name, new Hand(
                    playerToDiscard.Hand.Cards.Except(new[] {cardToDiscard})));
                newPlayer2 = currentTable.Players.ElementAt(1);
            }
            else
            {
                newPlayer1 = currentTable.Players.ElementAt(0);
                newPlayer2 = new Player(playerToDiscard.Id, playerToDiscard.Name, new Hand(
                    playerToDiscard.Hand.Cards.Except(new[] {cardToDiscard})));
            }

            var newDiscardPile = new DiscardPile(currentGameState.Table.DiscardPile.Cards.Append(cardToDiscard));
            var newTable = new Table(new[] {newPlayer1, newPlayer2}, currentTable.Deck, newDiscardPile,
                currentGameState.Table.StockPile);

            var newTurn = new Turn(currentGameState.CurrentTurn.Number + 1, GetNextPlayer(currentGameState).Id);

            return new Result(true, ErrorKey.None,
                new InGameState(currentGameState.GameId, newTable, currentGameState.StartingPlayerId, newTurn));
        }

        public Result Laydown(InGameState currentGameState, Player playerToPlay, IEnumerable<Card> cards)
        {
            if (playerToPlay.Id != currentGameState.CurrentTurn.CurrentPlayerId)
                return new Result(false, ErrorKey.NotTurn, currentGameState);

            if (!currentGameState.CurrentTurn.HasTakenCard())
                return new Result(false, ErrorKey.InvalidAction, currentGameState);

            if (!playerToPlay.Hand.Contains(cards))
                return new Result(false, ErrorKey.InvalidAction, currentGameState);

            // Must be at least 3 Cards
            if (cards.Count() < 3)
                return new Result(false, ErrorKey.InvalidAction, currentGameState);

            // If Book, must all be same Rank
            if (cards.Select(x => x.Rank).Distinct().Count() == 1)
            {
                var newPlayer1 = currentGameState.Table.Players.ElementAt(0);
                var newPlayer2 = currentGameState.Table.Players.ElementAt(1);

                var newTable = new Table(new[] { newPlayer1, newPlayer2 }, currentGameState.Table.Deck, currentGameState.Table.DiscardPile,
                    currentGameState.Table.StockPile, new IEnumerable<Card>[] { cards});

                return new Result(true, ErrorKey.None, new InGameState(currentGameState.GameId, newTable, currentGameState.StartingPlayerId, currentGameState.CurrentTurn));
            }

            // If Sequence, must all have same Suit
            if (cards.Select(x => x.Suit).Distinct().Count() == 1)
            {
                //var newPlayer1 = currentGameState.Table.Players.ElementAt(0);
                //var newPlayer2 = currentGameState.Table.Players.ElementAt(1);

                Player newPlayer1;
                Player newPlayer2;
                
                if (playerToPlay.Id == currentGameState.CurrentTurn.CurrentPlayerId)
                {
                    newPlayer1 = new Player(playerToPlay.Id, playerToPlay.Name, new Hand(
                        playerToPlay.Hand.Cards.Except(cards)));
                    newPlayer2 = currentGameState.Table.Players.ElementAt(1);
                }
                else
                {
                    newPlayer1 = currentGameState.Table.Players.ElementAt(0);
                    newPlayer2 = currentGameState.Table.Players.ElementAt(1);
                }

                var newTable = new Table(new[] { newPlayer1, newPlayer2 }, currentGameState.Table.Deck, currentGameState.Table.DiscardPile,
                    currentGameState.Table.StockPile, new IEnumerable<Card>[] { cards });

                return new Result(true, ErrorKey.None, new InGameState(currentGameState.GameId, newTable, currentGameState.StartingPlayerId, currentGameState.CurrentTurn));
            }

            return new Result(false, ErrorKey.InvalidAction, currentGameState);
        }

        private Player GetNextPlayer(InGameState inGameState)
        {
            if (inGameState.CurrentTurn.CurrentPlayerId == inGameState.Table.Players.ElementAt(1).Id)
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
using System.Linq;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class TakeCommand : GameCommand
    {
        private readonly CrazyEightsRules crazyEightsRules;
        private readonly CurrentTurnBuilder currentTurnBuilder;
        private readonly GameState gameState;
        private readonly IShuffler shuffler;
        private readonly TakeContext takeContext;

        public TakeCommand(CrazyEightsRules crazyEightsRules, IShuffler shuffler, GameState gameState,
            TakeContext takeContext)
        {
            this.crazyEightsRules = crazyEightsRules;
            this.shuffler = shuffler;
            this.gameState = gameState;
            this.takeContext = takeContext;

            currentTurnBuilder = new CurrentTurnBuilder(crazyEightsRules);
        }

        public override IsValidResult IsValid()
        {
            if (gameState.CurrentStateOfPlay.HasWinner)
                return new IsValidResult(false, CommandIsValidResultMessageKey.GameCompleted);

            var validPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch,
                takeContext.ExecutingPlayer.Hand,
                gameState.CurrentSelectedSuit,
                gameState.AnyPlaysOrTakes);

            if (validPlays)
                return new IsValidResult(false, CommandIsValidResultMessageKey.InvalidTake);

            if (gameState.CurrentPlayerToPlayNumber != takeContext.ExecutingPlayer.Number)
                return new IsValidResult(false, CommandIsValidResultMessageKey.NotPlayersTurn);

            return new IsValidResult(true, CommandIsValidResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var currentTable = gameState.CurrentTable;

            var takenCard = currentTable.MoveCardFromStockPileToPlayer(gameState.CurrentPlayerToPlay);
            gameState.EventLog.AddEvent(new Taken(gameState.EventLog.NextEventNumber,
                gameState.CurrentPlayerToPlayNumber, takenCard));

            if (currentTable.StockPile.IsEmpty())
                MoveDiscardPileToStockPile();

            StateOfTurn currentTurn;

            if (gameState.CurrentStateOfTurn.PreviousActions.Count == crazyEightsRules.NumberOfTakesBeforePass - 1)
            {
                gameState.EventLog.AddEvent(new Passed(gameState.EventLog.NextEventNumber,
                    gameState.CurrentPlayerToPlayNumber));
                gameState.EventLog.AddEvent(new TurnEnded(gameState.EventLog.NextEventNumber,
                    gameState.CurrentPlayerToPlayNumber));

                currentTurn = currentTurnBuilder.BuildNextTurn(gameState, gameState.NextPlayer,
                    gameState.CurrentSelectedSuit, takenCard);
            }
            else
            {
                currentTurn = currentTurnBuilder.AddTakenCard(gameState, takenCard);
            }

            gameState.CurrentStateOfTurn = currentTurn;

            return gameState;
        }

        private void MoveDiscardPileToStockPile()
        {
            var cardsToRemove = gameState.CurrentTable.DiscardPile.RestOfCards.Cards.ToArray();

            foreach (var card in cardsToRemove)
            {
                gameState.CurrentTable.MoveCardFromDiscardPileToStockPile();
                gameState.EventLog.AddEvent(new CardMoved(gameState.EventLog.NextEventNumber, card,
                    CardMoveSources.DiscardPile,
                    CardMoveSources.StockPile));
            }

            ShuffleStockPile();
        }

        private void ShuffleStockPile()
        {
            var startCards = gameState.CurrentTable.StockPile.Cards.ToArray();
            var shuffled = shuffler.Shuffle(new CardCollection(startCards));
            gameState.CurrentTable.StockPile = new StockPile(shuffled);
            gameState.EventLog.AddEvent(new Shuffled(gameState.EventLog.NextEventNumber,
                CardMoveSources.StockPile, new CardCollection(startCards), shuffled));
        }
    }
}
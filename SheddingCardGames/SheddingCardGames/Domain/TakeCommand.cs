using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
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
            var validPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch,
                takeContext.ExecutingPlayer.Hand,
                gameState.CurrentSelectedSuit,
                gameState.AnyPlaysOrTakes);

            if (validPlays)
                return new IsValidResult(false, CommandExecutionResultMessageKey.InvalidTake);

            if (gameState.CurrentPlayerToPlayNumber != takeContext.ExecutingPlayer.Number)
                return new IsValidResult(false, CommandExecutionResultMessageKey.NotPlayersTurn);

            return new IsValidResult(true, CommandExecutionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var currentTable = gameState.CurrentTable;

            var takenCard = currentTable.MoveCardFromStockPileToPlayer(gameState.CurrentPlayerToPlay);
            gameState.AddEvent(new Taken(gameState.NextEventNumber,
                gameState.CurrentPlayerToPlayNumber, takenCard));

            if (currentTable.StockPile.IsEmpty())
                MoveDiscardPileToStockPile();

            CurrentTurn currentTurn;

            if (gameState.CurrentTurn.PreviousActions.Count == crazyEightsRules.NumberOfTakesBeforePass - 1)
            {
                gameState.AddEvent(new Passed(gameState.NextEventNumber,
                    gameState.CurrentPlayerToPlayNumber));
                gameState.AddEvent(new TurnEnded(gameState.NextEventNumber,
                    gameState.CurrentPlayerToPlayNumber));

                currentTurn = currentTurnBuilder.BuildNextTurn(gameState, gameState.NextPlayer,
                    gameState.CurrentSelectedSuit, takenCard);
            }
            else
            {
                currentTurn = currentTurnBuilder.AddTakenCard(gameState, takenCard);
            }

            gameState.CurrentTurn = currentTurn;

            return gameState;
        }

        private void MoveDiscardPileToStockPile()
        {
            var cardsToRemove = gameState.CurrentTable.DiscardPile.RestOfCards.Cards.ToArray();

            foreach (var card in cardsToRemove)
            {
                gameState.CurrentTable.MoveCardFromDiscardPileToStockPile();
                gameState.AddEvent(new CardMoved(gameState.NextEventNumber, card,
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
            gameState.AddEvent(new Shuffled(gameState.NextEventNumber,
                CardMoveSources.StockPile, new CardCollection(startCards), shuffled));
        }
    }
}
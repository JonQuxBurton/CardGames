using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class SelectSuitCommand : GameCommand
    {
        private readonly CrazyEightsRules crazyEightsRules;
        private readonly GameState gameState;
        private readonly SelectSuitContext selectSuitContext;
        private readonly CurrentTurnBuilder currentTurnBuilder;

        public SelectSuitCommand(CrazyEightsRules crazyEightsRules, GameState gameState, SelectSuitContext selectSuitContext)
        {
            this.crazyEightsRules = crazyEightsRules;
            this.gameState = gameState;
            this.selectSuitContext = selectSuitContext;

            currentTurnBuilder = new CurrentTurnBuilder(crazyEightsRules);
        }

        public override IsValidResult IsValid()
        {
            if (gameState.CurrentPlayerToPlayNumber != selectSuitContext.ExecutingPlayer.Number)
                return new IsValidResult(false, CommandIsValidResultMessageKey.NotPlayersTurn);

            if (!gameState.AnyPlaysOrTakes)
                return new IsValidResult(false, CommandIsValidResultMessageKey.InvalidPlay);

            if (!crazyEightsRules.IsAlwaysValidCard(gameState.CurrentCardToMatch))
                return new IsValidResult(false, CommandIsValidResultMessageKey.InvalidPlay);

            return new IsValidResult(true, CommandIsValidResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.CurrentTurn = currentTurnBuilder.BuildNextTurn(gameState, gameState.NextPlayer, selectSuitContext.SelectedSuit);
            gameState.AddEvent(new SuitSelected(gameState.NextEventNumber,
                selectSuitContext.ExecutingPlayer.Number, selectSuitContext.SelectedSuit));
            gameState.AddEvent(new TurnEnded(gameState.NextEventNumber, selectSuitContext.ExecutingPlayer.Number));

            return gameState;
        }
    }
}
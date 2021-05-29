using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class SelectSuitCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly SelectSuitContext selectSuitContext;
        private readonly TurnBuilder turnBuilder;

        public SelectSuitCommand(IRules rules, GameState gameState, SelectSuitContext selectSuitContext)
        {
            this.gameState = gameState;
            this.selectSuitContext = selectSuitContext;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            if (gameState.CurrentPlayerToPlayNumber != selectSuitContext.ExecutingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!gameState.AnyPlaysOrTakes)
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            if (gameState.CurrentCardToMatch.Rank != 8)
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.AddEvent(new SuitSelected(gameState.NextEventNumber,
                selectSuitContext.ExecutingPlayer.Number, selectSuitContext.SelectedSuit));

            gameState.CurrentTurn = turnBuilder.BuildNextTurn(gameState, gameState.NextPlayer, selectSuitContext.SelectedSuit);

            return gameState;
        }
    }
}
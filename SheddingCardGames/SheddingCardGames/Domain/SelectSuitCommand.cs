using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class SelectSuitCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly Player executingPlayer;
        private readonly Suit selectedSuit;
        private readonly TurnBuilder turnBuilder;

        public SelectSuitCommand(IRules rules, GameState gameState, Player executingPlayer, Suit selectedSuit)
        {
            this.gameState = gameState;
            this.executingPlayer = executingPlayer;
            this.selectedSuit = selectedSuit;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            if (gameState.CurrentPlayerToPlayNumber != executingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (gameState.CurrentCardToMatch.Rank != 8)
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.Events.Add(new SuitSelected(gameState.NextEventNumber,
                executingPlayer.Number, selectedSuit));

            gameState.PreviousTurnResult = new PreviousTurnResult(false, null, selectedSuit);
            gameState.CurrentTurn = turnBuilder.BuildNextTurn(gameState, gameState.NextPlayer, selectedSuit);

            return gameState;
        }
    }
}
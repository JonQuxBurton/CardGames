using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class SelectSuitCommand : GameCommand
    {
        private readonly SelectSuitCommandContext context;
        private readonly GameState currentGameState;
        private readonly IRules rules;
        private readonly TurnBuilder turnBuilder;

        public SelectSuitCommand(IRules rules, GameState currentGameState, SelectSuitCommandContext context)
        {
            this.rules = rules;
            this.currentGameState = currentGameState;
            this.context = context;

            turnBuilder = new TurnBuilder(rules);
        }

        private Player NextPlayer
        {
            get
            {
                var nextPlayerNumber = currentGameState.CurrentTurn.PlayerToPlay.Number + 1;
                if (nextPlayerNumber > currentGameState.CurrentTable.Players.Count)
                    nextPlayerNumber = 1;

                return currentGameState.CurrentTable.Players[nextPlayerNumber - 1];
            }
        }

        public override ActionResult IsValid()
        {
            if (currentGameState.CurrentTurn.PlayerToPlay.Number != context.ExecutingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (currentGameState.CurrentTable.DiscardPile.CardToMatch.Rank != 8)
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            currentGameState.Events.Add(new SuitSelected(GetNextEventNumber(currentGameState.Events),
                context.ExecutingPlayer.Number, context.SelectedSuit));

            currentGameState.PreviousTurnResult = new PreviousTurnResult(false, null, context.SelectedSuit);
            currentGameState.CurrentTurn = turnBuilder.Build(currentGameState, NextPlayer, context.SelectedSuit);

            return currentGameState;
        }
    }
}
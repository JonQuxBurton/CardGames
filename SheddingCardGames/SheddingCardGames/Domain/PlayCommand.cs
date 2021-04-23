using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class PlayCommand : GameCommand
    {
        private readonly PlayCommandContext context;
        private readonly GameState currentGameState;
        private readonly Player executingPlayer;
        private readonly IRules rules;
        private readonly TurnBuilder turnBuilder;

        public PlayCommand(Player executingPlayer, IRules rules, GameState currentGameState, PlayCommandContext context)
        {
            this.executingPlayer = executingPlayer;
            this.rules = rules;
            this.currentGameState = currentGameState;
            this.context = context;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            if (executingPlayer.Number != context.Player.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!executingPlayer.Hand.Contains(context.PlayedCard))
                return new ActionResult(false, ActionResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay())
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            currentGameState.CurrentTable.MoveCardFromPlayerToDiscardPile(context.Player, context.PlayedCard);
            currentGameState.Events.Add(new Played(GetNextEventNumber(currentGameState.Events), context.Player.Number,
                context.PlayedCard));

            // TODO
            // GetPlayType - Won | CrazyEight | Standard
            // ProcessPlay(PlayTypes['Winner'])

            if (IsWinner())
            {
                currentGameState.Events.Add(new RoundWon(GetNextEventNumber(currentGameState.Events), context.Player.Number));
                currentGameState.PreviousTurnResult = new PreviousTurnResult(true, context.Player);
                currentGameState.CurrentTurn = turnBuilder.BuildWinningTurn(currentGameState);
            }
            else if (context.PlayedCard.Rank == 8)
            {
                currentGameState.PreviousTurnResult = new PreviousTurnResult(false);
                currentGameState.CurrentTurn = turnBuilder.BuildCrazyEightTurn(currentGameState);
            }
            else
            {
                currentGameState.PreviousTurnResult = new PreviousTurnResult(false);
                currentGameState.CurrentTurn = turnBuilder.BuildNextTurn(currentGameState, currentGameState.NextPlayer);
            }

            return currentGameState;
        }

        private bool IsValidPlay()
        {
            var selectedSuit = currentGameState.PreviousTurnResult?.SelectedSuit;
            return rules.IsValidPlay(context.PlayedCard, currentGameState.CurrentTable.DiscardPile.CardToMatch,
                currentGameState.CurrentTurn.TurnNumber,
                selectedSuit);
        }

        private bool IsWinner()
        {
            return context.Player.Hand.IsEmpty();
        }
    }
}
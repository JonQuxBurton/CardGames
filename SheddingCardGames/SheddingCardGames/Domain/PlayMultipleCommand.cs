using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class PlayMultipleCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly PlayMultipleContext playContext;
        private readonly IRules rules;
        private readonly TurnBuilder turnBuilder;

        public PlayMultipleCommand(IRules rules, GameState gameState, PlayMultipleContext playContext)
        {
            this.rules = rules;
            this.gameState = gameState;
            this.playContext = playContext;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            if (playContext.ExecutingPlayer.Number != gameState.CurrentPlayerToPlayNumber)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!playContext.ExecutingPlayer.Hand.ContainsAll(playContext.PlayedCards))
                return new ActionResult(false, ActionResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay())
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.CurrentTable.MoveCardsFromPlayerToDiscardPile(playContext.ExecutingPlayer, playContext.PlayedCards);

            gameState.AddEvent(new Played(gameState.NextEventNumber, playContext.ExecutingPlayer.Number,
                playContext.PlayedCards.ToArray()));

            //if (HasWon())
            //{
            //    gameState.AddEvent(new RoundWon(gameState.NextEventNumber, playContext.ExecutingPlayer.Number));
            //    gameState.PreviousTurnResult = new PreviousTurnResult(true, playContext.ExecutingPlayer);
            //    gameState.CurrentTurn = turnBuilder.BuildWinningTurn(gameState);
            //}
            //else if (playContext.PlayedCard.Rank == 8)
            //{
            //    gameState.PreviousTurnResult = new PreviousTurnResult(false);
            //    gameState.CurrentTurn = turnBuilder.BuildCrazyEightTurn(gameState);
            //}
            //else
            //{
            gameState.PreviousTurnResult = new PreviousTurnResult(false);
            gameState.CurrentTurn = turnBuilder.BuildNextTurn(gameState, gameState.NextPlayer);
            //}

            return gameState;
        }

        private bool IsValidPlay()
        {
            return playContext.PlayedCards.All(x => rules.IsValidPlay(x,
                gameState.CurrentCardToMatch,
                gameState.CurrentTurnNumber,
                gameState.CurrentSelectedSuit));
        }

        private bool HasWon()
        {
            return playContext.ExecutingPlayer.Hand.IsEmpty();
        }
    }
}
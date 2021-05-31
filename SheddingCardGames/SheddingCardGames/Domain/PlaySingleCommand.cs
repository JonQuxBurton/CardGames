using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class PlaySingleCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly PlayContext playContext;
        private readonly CrazyEightsRules crazyEightsRules;
        private readonly CurrentTurnBuilder currentTurnBuilder;

        public PlaySingleCommand(CrazyEightsRules crazyEightsRules, GameState gameState, PlayContext playContext)
        {
            this.crazyEightsRules = crazyEightsRules;
            this.gameState = gameState;
            this.playContext = playContext;

            currentTurnBuilder = new CurrentTurnBuilder(crazyEightsRules);
        }

        public override ActionResult IsValid()
        {
            if (playContext.ExecutingPlayer.Number != gameState.CurrentPlayerToPlayNumber)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!playContext.ExecutingPlayer.Hand.ContainsAll(playContext.CardsPlayed))
                return new ActionResult(false, ActionResultMessageKey.CardIsNotInPlayersHand);

            if (playContext.CardsPlayed.Count > 1)
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            if (!IsValidPlay())
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.CurrentTable.MoveCardsFromPlayerToDiscardPile(playContext.ExecutingPlayer, playContext.CardsPlayed);

            gameState.AddEvent(new Played(gameState.NextEventNumber, playContext.ExecutingPlayer.Number,
                playContext.CardsPlayed.ToArray()));

            if (HasWon())
            {
                gameState.CurrentTurn = currentTurnBuilder.BuildWinningTurn(gameState, playContext.ExecutingPlayer);
                gameState.AddEvent(new RoundWon(gameState.NextEventNumber, playContext.ExecutingPlayer.Number));
            }
            else if (playContext.CardsPlayed.First().Rank == 8)
            {
                gameState.CurrentTurn = currentTurnBuilder.BuildCrazyEightTurn(gameState);
            }
            else
            {
                gameState.CurrentTurn = currentTurnBuilder.BuildNextTurn(gameState, gameState.NextPlayer);
                gameState.AddEvent(new TurnEnded(gameState.NextEventNumber, playContext.ExecutingPlayer.Number));
            }

            return gameState;
        }

        private bool IsValidPlay()
        {
            return crazyEightsRules.IsValidPlay(playContext.CardsPlayed, gameState.CurrentCardToMatch,
                gameState.CurrentSelectedSuit, gameState.AnyPlaysOrTakes);
        }

        private bool HasWon()
        {
            return playContext.ExecutingPlayer.Hand.IsEmpty();
        }
    }
}
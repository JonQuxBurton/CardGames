using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class PlayCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly PlayContext playContext;
        private readonly CrazyEightsRules crazyEightsRules;
        private readonly CurrentTurnBuilder currentTurnBuilder;

        public PlayCommand(CrazyEightsRules crazyEightsRules, GameState gameState, PlayContext playContext)
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
                gameState.AddEvent(new RoundWon(gameState.NextEventNumber, playContext.ExecutingPlayer.Number));
                gameState.CurrentTurn = currentTurnBuilder.BuildWinningTurn(gameState, playContext.ExecutingPlayer);
            }
            else if (playContext.CardsPlayed.First().Rank == 8)
            {
                gameState.CurrentTurn = currentTurnBuilder.BuildCrazyEightTurn(gameState);
            }
            else
            {
                gameState.CurrentTurn = currentTurnBuilder.BuildNextTurn(gameState, gameState.NextPlayer);
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
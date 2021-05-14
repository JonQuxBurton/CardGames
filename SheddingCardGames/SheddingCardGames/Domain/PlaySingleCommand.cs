using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class PlaySingleCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly PlayContext playContext;
        private readonly IRules rules;
        private readonly TurnBuilder turnBuilder;

        public PlaySingleCommand(IRules rules, GameState gameState, PlayContext playContext)
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
                gameState.AddEvent(new RoundWon(gameState.NextEventNumber, playContext.ExecutingPlayer.Number));
                gameState.PreviousTurnResult = new PreviousTurnResult(true, playContext.ExecutingPlayer);
                gameState.CurrentTurn = turnBuilder.BuildWinningTurn(gameState);
            }
            else if (playContext.CardsPlayed.First().Rank == 8)
            {
                gameState.PreviousTurnResult = new PreviousTurnResult(false);
                gameState.CurrentTurn = turnBuilder.BuildCrazyEightTurn(gameState);
            }
            else
            {
                gameState.PreviousTurnResult = new PreviousTurnResult(false);
                gameState.CurrentTurn = turnBuilder.BuildNextTurn(gameState, gameState.NextPlayer);
            }

            return gameState;
        }

        private bool IsValidPlay()
        {
            return rules.IsValidPlay(playContext.CardsPlayed, gameState.CurrentCardToMatch,
                gameState.CurrentSelectedSuit, gameState.AnyPlaysOrTakes);
        }

        private bool HasWon()
        {
            return playContext.ExecutingPlayer.Hand.IsEmpty();
        }
    }
}
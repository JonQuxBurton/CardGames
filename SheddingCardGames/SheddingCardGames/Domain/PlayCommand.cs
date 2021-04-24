using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class PlayCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly Card playedCard;
        private readonly Player executingPlayer;
        private readonly IRules rules;
        private readonly TurnBuilder turnBuilder;

        public PlayCommand(Player executingPlayer, IRules rules, GameState gameState, Card playedCard)
        {
            this.executingPlayer = executingPlayer;
            this.rules = rules;
            this.gameState = gameState;
            this.playedCard = playedCard;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            if (executingPlayer.Number != gameState.CurrentPlayerToPlayNumber)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!executingPlayer.Hand.Contains(playedCard))
                return new ActionResult(false, ActionResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay())
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.CurrentTable.MoveCardFromPlayerToDiscardPile(executingPlayer, playedCard);
            gameState.Events.Add(new Played(gameState.NextEventNumber, executingPlayer.Number,
                playedCard));

            if (HasWon())
            {
                gameState.Events.Add(new RoundWon(gameState.NextEventNumber, executingPlayer.Number));
                gameState.PreviousTurnResult = new PreviousTurnResult(true, executingPlayer);
                gameState.CurrentTurn = turnBuilder.BuildWinningTurn(gameState);
            }
            else if (playedCard.Rank == 8)
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
            return rules.IsValidPlay(playedCard, 
                gameState.CurrentCardToMatch,
                gameState.CurrentTurnNumber,
                gameState.CurrentSelectedSuit);
        }

        private bool HasWon()
        {
            return executingPlayer.Hand.IsEmpty();
        }
    }
}
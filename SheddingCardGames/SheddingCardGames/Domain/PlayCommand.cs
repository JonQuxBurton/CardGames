using System.Linq;
using SheddingCardGames.Domain.Events;

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

        public override IsValidResult IsValid()
        {
            if (gameState.CurrentStateOfPlay.HasWinner)
                return new IsValidResult(false, CommandIsValidResultMessageKey.GameCompleted);

            if (playContext.ExecutingPlayer.Number != gameState.CurrentPlayerToPlayNumber)
                return new IsValidResult(false, CommandIsValidResultMessageKey.NotPlayersTurn);

            if (!playContext.ExecutingPlayer.Hand.ContainsAll(playContext.CardsPlayed))
                return new IsValidResult(false, CommandIsValidResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay())
                return new IsValidResult(false, CommandIsValidResultMessageKey.InvalidPlay);

            return new IsValidResult(true, CommandIsValidResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.CurrentTable.MoveCardsFromPlayerToDiscardPile(playContext.ExecutingPlayer, playContext.CardsPlayed);

            gameState.EventLog.AddEvent(new Played(gameState.EventLog.NextEventNumber, playContext.ExecutingPlayer.Number,
                playContext.CardsPlayed.ToArray()));

            if (HasWon())
                Won();
            else if (crazyEightsRules.IsAlwaysValidCard(playContext.CardsPlayed.First()))
                PlayedAlwaysValidCard();
            else
                TurnEnded();

            return gameState;
        }

        private void TurnEnded()
        {
            gameState.CurrentStateOfTurn = currentTurnBuilder.BuildNextTurn(gameState, gameState.NextPlayer);
            gameState.EventLog.AddEvent(new TurnEnded(gameState.EventLog.NextEventNumber, playContext.ExecutingPlayer.Number));
        }

        private void PlayedAlwaysValidCard()
        {
            gameState.CurrentStateOfTurn = currentTurnBuilder.BuildCrazyEightTurn(gameState);
        }

        private void Won()
        {
            gameState.CurrentStateOfPlay =
                StateOfPlay.WithWinner(gameState.CurrentStateOfPlay, playContext.ExecutingPlayer);
            //gameState.CurrentStateOfPlay.Won(playContext.ExecutingPlayer);
            gameState.CurrentStateOfTurn = currentTurnBuilder.BuildWinningTurn(gameState, playContext.ExecutingPlayer);
            gameState.EventLog.AddEvent(new RoundWon(gameState.EventLog.NextEventNumber, playContext.ExecutingPlayer.Number));
        }

        private bool IsValidPlay()
        {
            return crazyEightsRules.IsValidPlay(new IsValidPlayContext(playContext.CardsPlayed, gameState.CurrentCardToMatch, gameState.CurrentSelectedSuit, gameState.AnyPlaysOrTakes));
        }

        private bool HasWon()
        {
            return playContext.ExecutingPlayer.Hand.IsEmpty();
        }
    }
}
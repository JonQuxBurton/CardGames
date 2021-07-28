using System.Linq;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain.Whist
{
    public class PlayCommand : GameCommand
    {
        private readonly Rules rules;
        private readonly GameState gameState;
        private readonly PlayContext playContext;
        private readonly CurrentTrickBuilder currentTrickBuilder;

        public PlayCommand(Rules rules, GameState gameState, PlayContext playContext)
        {
            this.rules = rules;
            this.gameState = gameState;
            this.playContext = playContext;

            currentTrickBuilder = new CurrentTrickBuilder();
        }

        public override IsValidResult IsValid()
        {
            if (gameState.CurrentStateOfPlay.HasWinner)
                return new IsValidResult(false, CommandIsValidResultMessageKey.GameCompleted);

            if (playContext.ExecutingPlayer.Number != gameState.CurrentPlayerToPlayNumber)
                return new IsValidResult(false, CommandIsValidResultMessageKey.NotPlayersTurn);

            if (!playContext.ExecutingPlayer.Hand.Contains(playContext.CardPlayed))
                return new IsValidResult(false, CommandIsValidResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay())
                return new IsValidResult(false, CommandIsValidResultMessageKey.InvalidPlay);

            return new IsValidResult(true, CommandIsValidResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.CurrentTable.MoveCardFromPlayerToTrick(playContext.ExecutingPlayer, playContext.CardPlayed);

            gameState.EventLog.AddEvent(new Played(gameState.EventLog.NextEventNumber, playContext.ExecutingPlayer.Number,
                new[] { playContext.CardPlayed }));

            gameState.CurrentStateOfTrick = currentTrickBuilder.AddCard(gameState, playContext.CardPlayed, gameState.NextPlayer);

            return gameState;
        }

        private bool IsValidPlay()
        {
            return rules.IsValidPlay(new IsValidPlayContext(playContext.CardPlayed, playContext.ExecutingPlayer.Hand, gameState.CurrentStateOfTrick));
        }
    }
}
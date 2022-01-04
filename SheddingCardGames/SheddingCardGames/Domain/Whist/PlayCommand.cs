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
            if (gameState.CurrentTable.Trick.CardCollection.IsEmpty())
                gameState.EventLog.AddEvent(new TrickStarted(gameState.EventLog.NextEventNumber));

            gameState.CurrentTable.MoveCardFromPlayerToTrick(playContext.ExecutingPlayer, playContext.CardPlayed);

            gameState.EventLog.AddEvent(new Played(gameState.EventLog.NextEventNumber, playContext.ExecutingPlayer.Number,
                new[] { playContext.CardPlayed }));

            //var highestCard = gameState.CurrentStateOfTrick.CardsPlayed.OrderBy(x => x.Rank).First();
            //gameState.CurrentStateOfTrick.

            gameState.CurrentStateOfTrick = currentTrickBuilder.AddCard(gameState, playContext.CardPlayed, gameState.NextPlayer);
            gameState.CurrentStateOfTrick = currentTrickBuilder.AddWinner(gameState, playContext.CardPlayed, gameState.NextPlayer, playContext.ExecutingPlayer);

            if (gameState.CurrentTable.Trick.IsCompleted)
            {
                gameState.EventLog.AddEvent(new TrickCompleted(gameState.EventLog.NextEventNumber, playContext.ExecutingPlayer.Number));
            }
                

            return gameState;
        }

        private bool IsValidPlay()
        {
            return rules.IsValidPlay(new IsValidPlayContext(playContext.CardPlayed, playContext.ExecutingPlayer.Hand, gameState.CurrentStateOfTrick));
        }
    }
}
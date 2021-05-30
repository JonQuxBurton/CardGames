using System.Collections.Immutable;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class CurrentTurnBuilder
    {
        private readonly CrazyEightsRules crazyEightsRules;

        public CurrentTurnBuilder(CrazyEightsRules crazyEightsRules)
        {
            this.crazyEightsRules = crazyEightsRules;
        }

        public CurrentTurn BuildFirstTurn(GameState gameState, Player nextPlayer)
        {
            var hasValidPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, null, gameState.AnyPlaysOrTakes);

            return new CurrentTurn(1,
                nextPlayer,
                GetNextAction(hasValidPlays));
        }

        public CurrentTurn BuildNextTurn(GameState gameState, Player nextPlayer, Suit? selectedSuit = null, Card takenCard = null)
        {
            var nextTurnNumber = gameState.CurrentTurnNumber + 1;

            var hasValidPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, selectedSuit, gameState.AnyPlaysOrTakes);

            return new CurrentTurn(nextTurnNumber,
                nextPlayer,
                GetNextAction(hasValidPlays),
                takenCard,
                selectedSuit);
        }

        public CurrentTurn AddTakenCard(GameState gameState, Card takenCard)
        {
            var hasValidPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch, gameState.CurrentPlayerToPlay.Hand, gameState.CurrentSelectedSuit, gameState.AnyPlaysOrTakes);
            
            return new CurrentTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                GetNextAction(hasValidPlays),
                takenCard,
                gameState.CurrentSelectedSuit,
                null,
                gameState.CurrentTurn.PreviousActions.Add(Action.Take));
        }

        public CurrentTurn BuildCrazyEightTurn(GameState gameState)
        {
            return new CurrentTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                Action.SelectSuit, null, null, null, ImmutableList.Create(Action.Play));
        }

        public CurrentTurn BuildWinningTurn(GameState gameState, Player winner)
        {
            return new CurrentTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                Action.Won, 
                null, 
                null,
                winner,
                ImmutableList.Create(Action.Play));
        }
        
        private Action GetNextAction(bool hasValidPlays)
        {
            return !hasValidPlays ? Action.Take : Action.Play;
        }
    }
}
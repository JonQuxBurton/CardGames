using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public class CurrentTurnBuilder
    {
        private readonly CrazyEightsRules crazyEightsRules;

        public CurrentTurnBuilder(CrazyEightsRules crazyEightsRules)
        {
            this.crazyEightsRules = crazyEightsRules;
        }

        public StateOfTurn BuildFirstTurn(GameState gameState, Player nextPlayer)
        {
            var hasValidPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, null, false);

            return new StateOfTurn(1,
                nextPlayer,
                GetNextAction(hasValidPlays));
        }

        public StateOfTurn BuildNextTurn(GameState gameState, Player nextPlayer, Suit? selectedSuit = null, Card takenCard = null)
        {
            var nextTurnNumber = gameState.CurrentTurnNumber + 1;

            var hasValidPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, selectedSuit, gameState.CurrentStateOfPlay.AnyPlaysOrTakes);

            return new StateOfTurn(nextTurnNumber,
                nextPlayer,
                GetNextAction(hasValidPlays),
                takenCard,
                selectedSuit);
        }

        public StateOfTurn AddTakenCard(GameState gameState, Card takenCard)
        {
            var hasValidPlays = crazyEightsRules.HasValidPlay(gameState.CurrentCardToMatch, gameState.CurrentPlayerToPlay.Hand, gameState.CurrentSelectedSuit, gameState.CurrentStateOfPlay.AnyPlaysOrTakes);
            
            return new StateOfTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                GetNextAction(hasValidPlays),
                takenCard,
                gameState.CurrentSelectedSuit,
                gameState.CurrentStateOfTurn.PreviousActions.Add(Action.Take));
        }

        public StateOfTurn BuildCrazyEightTurn(GameState gameState)
        {
            return new StateOfTurn(
                gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                Action.SelectSuit, 
                null, 
                null, 
                ImmutableList.Create(Action.Play));
        }

        public StateOfTurn BuildWinningTurn(GameState gameState, Player winner)
        {
            return new StateOfTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                Action.Won, 
                null, 
                null,
                ImmutableList.Create(Action.Play));
        }
        
        private Action GetNextAction(bool hasValidPlays)
        {
            return !hasValidPlays ? Action.Take : Action.Play;
        }
    }
}
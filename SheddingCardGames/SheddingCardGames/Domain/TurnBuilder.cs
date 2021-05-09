using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class TurnBuilder
    {
        private readonly IRules rules;

        public TurnBuilder(IRules rules)
        {
            this.rules = rules;
        }

        public CurrentTurn BuildFirstTurn(GameState gameState, Player nextPlayer)
        {
            var hasValidPlays = rules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, 1, null);

            return new CurrentTurn(1,
                nextPlayer,
                GetNextAction(hasValidPlays));
        }

        public CurrentTurn BuildNextTurn(GameState gameState, Player nextPlayer, Suit? selectedSuit = null)
        {
            var nextTurnNumber = gameState.CurrentTurnNumber + 1;

            var currentTurnNumber = 1;
            if (gameState.CurrentTurn != null)
                currentTurnNumber = gameState.CurrentTurnNumber;

            var hasValidPlays = rules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, currentTurnNumber, selectedSuit);

            return new CurrentTurn(nextTurnNumber,
                nextPlayer,
                GetNextAction(hasValidPlays));
        }

        public CurrentTurn BuildCrazyEightTurn(GameState gameState)
        {
            return new CurrentTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                Action.SelectSuit);
        }

        public CurrentTurn BuildWinningTurn(GameState gameState)
        {
            return new CurrentTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                Action.Won);
        }
        
        private Action GetNextAction(bool hasValidPlays)
        {
            return !hasValidPlays ? Action.Take : Action.Play;
        }
    }
}
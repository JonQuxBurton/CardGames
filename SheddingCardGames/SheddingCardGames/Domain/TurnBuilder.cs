using System.Collections.Immutable;
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
            var hasValidPlays = rules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, null, gameState.AnyPlaysOrTakes);

            return new CurrentTurn(1,
                nextPlayer,
                GetNextAction(hasValidPlays));
        }

        public CurrentTurn BuildNextTurn(GameState gameState, Player nextPlayer, Suit? selectedSuit = null, Card takenCard = null)
        {
            var nextTurnNumber = gameState.CurrentTurnNumber + 1;

            var hasValidPlays = rules.HasValidPlay(gameState.CurrentCardToMatch, nextPlayer.Hand, selectedSuit, gameState.AnyPlaysOrTakes);

            return new CurrentTurn(nextTurnNumber,
                nextPlayer,
                GetNextAction(hasValidPlays),
                takenCard,
                selectedSuit);
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
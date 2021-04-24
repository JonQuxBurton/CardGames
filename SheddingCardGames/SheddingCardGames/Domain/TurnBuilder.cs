using System.Collections.Generic;
using System.Linq;
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
            return new CurrentTurn(1,
                nextPlayer,
                GetValidPlays(gameState, nextPlayer, null, 1),
                GetNextAction(GetValidPlays(gameState, nextPlayer, null, 1)));
        }

        public CurrentTurn BuildNextTurn(GameState gameState, Player nextPlayer, Suit? selectedSuit = null)
        {
            var nextTurnNumber = gameState.CurrentTurnNumber + 1;

            var currentTurnNumber = 1;
            if (gameState.CurrentTurn != null)
                currentTurnNumber = gameState.CurrentTurnNumber;

            return new CurrentTurn(nextTurnNumber,
                nextPlayer,
                GetValidPlays(gameState, nextPlayer, selectedSuit, currentTurnNumber),
                GetNextAction(GetValidPlays(gameState, nextPlayer, selectedSuit, currentTurnNumber)));
        }

        public CurrentTurn BuildCrazyEightTurn(GameState gameState)
        {
            return new CurrentTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                new Card[0],
                Action.SelectSuit);
        }

        public CurrentTurn BuildWinningTurn(GameState gameState)
        {
            return new CurrentTurn(gameState.CurrentTurnNumber,
                gameState.CurrentPlayerToPlay,
                new Card[0],
                Action.Won);
        }

        private Card[] GetValidPlays(GameState gameState, Player nextPlayer, Suit? selectedSuit, int currentTurnNumber)
        {
            var validPlays = rules
                .GetValidPlays(gameState.CurrentCardToMatch, nextPlayer.Hand, currentTurnNumber, selectedSuit)
                .ToArray();
            return validPlays;
        }

        private Action GetNextAction(IEnumerable<Card> validPlays)
        {
            return !validPlays.Any() ? Action.Take : Action.Play;
        }
    }
}
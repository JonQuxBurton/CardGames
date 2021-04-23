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

        public CurrentTurn BuildFirstTurn(GameState currentGameState, Player nextPlayer)
        {
            int nextTurnNumber = 1;
            var currentTurnNumber = 1;
            
            var validPlays = GetValidPlays(nextPlayer.Hand, currentGameState.CurrentTable.DiscardPile.CardToMatch, currentTurnNumber, null)
                .ToArray();

            var newTurn =
                new CurrentTurn(nextTurnNumber,
                    nextPlayer,
                    validPlays,
                    GetNextAction(validPlays));

            return newTurn;
        }
        
        public CurrentTurn BuildNextTurn(GameState currentGameState, Player nextPlayer, Suit? selectedSuit = null)
        {
            var nextTurnNumber = currentGameState.CurrentTurn.TurnNumber + 1;

            var currentTurnNumber = 1;
            if (currentGameState.CurrentTurn != null)
                currentTurnNumber = currentGameState.CurrentTurn.TurnNumber;

            var validPlays = GetValidPlays(nextPlayer.Hand, currentGameState.CurrentTable.DiscardPile.CardToMatch, currentTurnNumber, selectedSuit)
                .ToArray();

            var newTurn =
                new CurrentTurn(nextTurnNumber,
                    nextPlayer,
                    validPlays,
                    GetNextAction(validPlays));

            return newTurn;
        }

        public CurrentTurn BuildCrazyEightTurn(GameState currentGameState)
        {
            var newTurn =
                new CurrentTurn(currentGameState.CurrentTurn.TurnNumber,
                    currentGameState.CurrentTurn.PlayerToPlay,
                    new Card[0],
                    Action.SelectSuit);
            return newTurn;
        }

        public CurrentTurn BuildWinningTurn(GameState currentGameState)
        {
            var currentTurn = currentGameState.CurrentTurn;

            var newTurn =
                new CurrentTurn(currentTurn.TurnNumber,
                    currentTurn.PlayerToPlay,
                    new Card[0],
                    Action.Won);
            return newTurn;
        }

        private IEnumerable<Card> GetValidPlays(CardCollection hand, Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, hand, turnNumber, selectedSuit);
        }

        private Action GetNextAction(IEnumerable<Card> validPlays)
        {
            return !validPlays.Any() ? Action.Take : Action.Play;
        }
    }
}
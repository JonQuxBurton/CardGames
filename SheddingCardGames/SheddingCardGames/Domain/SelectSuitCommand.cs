using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class SelectSuitCommand : GameCommand
    {
        private readonly SelectSuitCommandContext context;
        private readonly GameState currentGameState;
        private readonly IRules rules;

        public SelectSuitCommand(IRules rules, GameState currentGameState, SelectSuitCommandContext context)
        {
            this.rules = rules;
            this.currentGameState = currentGameState;
            this.context = context;
        }

        public override ActionResult IsValid()
        {
            if (currentGameState.PlayerToPlay.Number != context.ExecutingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (currentGameState.CurrentTable.DiscardPile.CardToMatch.Rank != 8)
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            currentGameState.SelectedSuit = context.SelectedSuit;
            currentGameState.Events.Add(new SuitSelected(GetNextEventNumber(currentGameState.Events),
                context.ExecutingPlayer.Number, context.SelectedSuit));

            AddNextTurn(NextPlayer, currentGameState.SelectedSuit);

            return currentGameState;
        }

        private Player NextPlayer
        {
            get
            {
                var nextPlayerNumber = currentGameState.PlayerToPlay.Number + 1;
                if (nextPlayerNumber > currentGameState.CurrentTable.Players.Count)
                    nextPlayerNumber = 1;
                
                return currentGameState.CurrentTable.Players[nextPlayerNumber-1];
            }
        }

        private void AddNextTurn(Player nextPlayer, Suit? selectedSuit = null)
        {
            var nextTurnNumber = currentGameState.TurnNumber + 1;
            var validPlays = GetValidPlays(nextPlayer.Hand, currentGameState.CurrentTable.DiscardPile.CardToMatch,
                    currentGameState.TurnNumber, selectedSuit)
                .ToArray();

            var newTurn =
                new CurrentTurn(nextTurnNumber,
                    nextPlayer,
                    validPlays,
                    false,
                    null,
                    GetNextAction(validPlays),
                    selectedSuit);
            currentGameState.TurnNumber = nextTurnNumber;
            currentGameState.PlayerToPlay = nextPlayer;
            currentGameState.CurrentTurn = newTurn;
        }

        private Action GetNextAction(IEnumerable<Card> validPlays)
        {
            return !validPlays.Any() ? Action.Take : Action.Play;
        }

        private IEnumerable<Card> GetValidPlays(CardCollection hand, Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, hand, turnNumber, selectedSuit);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class PlayCommand : GameCommand
    {
        private readonly PlayCommandContext context;
        private readonly GameState currentGameState;
        private readonly Player executingPlayer;
        private readonly IRules rules;

        public PlayCommand(Player executingPlayer, IRules rules, GameState currentGameState, PlayCommandContext context)
        {
            this.executingPlayer = executingPlayer;
            this.rules = rules;
            this.currentGameState = currentGameState;
            this.context = context;
        }

        public override ActionResult IsValid()
        {
            if (executingPlayer.Number != context.Player.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!executingPlayer.Hand.Contains(context.PlayedCard))
                return new ActionResult(false, ActionResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay())
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            currentGameState.CurrentTable.MoveCardFromPlayerToDiscardPile(context.Player, context.PlayedCard);
            currentGameState.Events.Add(new Played(GetNextEventNumber(currentGameState.Events), context.Player.Number,
                context.PlayedCard));

            if (IsWinner())
                currentGameState.Events.Add(new RoundWon(GetNextEventNumber(currentGameState.Events), context.Player.Number));

            if (IsWinner())
                AddWinningTurn();
            else if (context.PlayedCard.Rank == 8)
                AddCrazyEightTurn();
            else
                AddNextTurn(NextPlayer);

            return currentGameState;
        }

        private void AddCrazyEightTurn()
        {
            var newTurn =
                new CurrentTurn(currentGameState.TurnNumber,
                    currentGameState.CurrentTurn.PlayerToPlay,
                    new Card[0],
                    false,
                    null,
                    Action.SelectSuit, null);
            currentGameState.CurrentTurn = newTurn;
        }

        private void AddWinningTurn()
        {
            var currentTurn = currentGameState.CurrentTurn;

            var newTurn =
                new CurrentTurn(currentTurn.TurnNumber,
                    currentGameState.PlayerToPlay,
                    new Card[0],
                    true,
                    currentGameState.PlayerToPlay,
                    Action.Won, null);
            currentGameState.CurrentTurn = newTurn;
        }

        private bool IsValidPlay()
        {
            var selectedSuit = currentGameState.SelectedSuit;
            return rules.IsValidPlay(context.PlayedCard, currentGameState.CurrentTable.DiscardPile.CardToMatch,
                currentGameState.TurnNumber,
                selectedSuit);
        }

        private bool IsWinner()
        {
            return context.Player.Hand.IsEmpty();
        }

        private Player NextPlayer
        {
            get
            {
                var nextPlayerNumber = currentGameState.PlayerToPlay.Number + 1;
                if (nextPlayerNumber > currentGameState.CurrentTable.Players.Count)
                    nextPlayerNumber = 1;

                return currentGameState.CurrentTable.Players[nextPlayerNumber - 1];
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
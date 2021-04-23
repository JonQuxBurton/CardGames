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
        private readonly TurnBuilder turnBuilder;

        public PlayCommand(Player executingPlayer, IRules rules, GameState currentGameState, PlayCommandContext context)
        {
            this.executingPlayer = executingPlayer;
            this.rules = rules;
            this.currentGameState = currentGameState;
            this.context = context;

            turnBuilder = new TurnBuilder(rules);
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
            {
                currentGameState.Events.Add(new RoundWon(GetNextEventNumber(currentGameState.Events), context.Player.Number));
                currentGameState.PreviousTurnResult = new PreviousTurnResult(true, context.Player);
                AddWinningTurn();
            }
            else if (context.PlayedCard.Rank == 8)
            {
                currentGameState.PreviousTurnResult = new PreviousTurnResult(false);
                AddCrazyEightTurn();
            }
            else
            {
                currentGameState.PreviousTurnResult = new PreviousTurnResult(false);
                currentGameState.CurrentTurn = turnBuilder.Build(currentGameState, NextPlayer);
            }

            return currentGameState;
        }

        private void AddCrazyEightTurn()
        {
            var newTurn =
                new CurrentTurn(currentGameState.CurrentTurn.TurnNumber,
                    currentGameState.CurrentTurn.PlayerToPlay,
                    new Card[0],
                    Action.SelectSuit);
            currentGameState.CurrentTurn = newTurn;
        }

        private void AddWinningTurn()
        {
            var currentTurn = currentGameState.CurrentTurn;

            var newTurn =
                new CurrentTurn(currentTurn.TurnNumber,
                    currentTurn.PlayerToPlay,
                    new Card[0],
                    Action.Won);
            currentGameState.CurrentTurn = newTurn;
        }

        private bool IsValidPlay()
        {
            var selectedSuit = currentGameState.PreviousTurnResult?.SelectedSuit;
            return rules.IsValidPlay(context.PlayedCard, currentGameState.CurrentTable.DiscardPile.CardToMatch,
                currentGameState.CurrentTurn.TurnNumber,
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
                var nextPlayerNumber = currentGameState.CurrentTurn.PlayerToPlay.Number + 1;
                if (nextPlayerNumber > currentGameState.CurrentTable.Players.Count)
                    nextPlayerNumber = 1;

                return currentGameState.CurrentTable.Players[nextPlayerNumber - 1];
            }
        }
    }
}
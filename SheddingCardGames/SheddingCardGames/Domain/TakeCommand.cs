using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class TakeCommand : GameCommand
    {
        private readonly TakeCommandContext context;
        private readonly GameState currentGameState;
        private readonly IRules rules;
        private readonly IShuffler shuffler;

        public TakeCommand(IRules rules, IShuffler shuffler, GameState currentGameState, TakeCommandContext context)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.currentGameState = currentGameState;
            this.context = context;
        }

        public override ActionResult IsValid()
        {
            var validPlays = GetValidPlays(context.ExecutingPlayer.Hand, currentGameState.CurrentTable.DiscardPile.CardToMatch, currentGameState.CurrentTurn.TurnNumber, null)
                .ToArray();

            if (validPlays.Any())
                return new ActionResult(false, ActionResultMessageKey.InvalidTake);

            if (currentGameState.PlayerToPlay.Number != context.ExecutingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var takenCard = currentGameState.CurrentTable.MoveCardFromStockPileToPlayer(currentGameState.PlayerToPlay);
            currentGameState.Events.Add(new Taken(GetNextEventNumber(currentGameState.Events), currentGameState.PlayerToPlay.Number, takenCard));

            if (currentGameState.CurrentTable.StockPile.IsEmpty())
                MoveDiscardPileToStockPile();

            currentGameState.TakenCard = takenCard;

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

                return currentGameState.CurrentTable.Players[nextPlayerNumber - 1];
            }
        }

        private IEnumerable<Card> GetValidPlays(CardCollection hand, Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, hand, turnNumber, selectedSuit);
        }

        private void MoveDiscardPileToStockPile()
        {
            var cardsToRemove = currentGameState.CurrentTable.DiscardPile.RestOfCards.Cards.ToArray();

            foreach (var card in cardsToRemove)
            {
                currentGameState.CurrentTable.MoveCardFromDiscardPileToStockPile();
                currentGameState.Events.Add(new CardMoved(GetNextEventNumber(currentGameState.Events), card, CardMoveSources.DiscardPile,
                    CardMoveSources.StockPile));
            }

            ShuffleStockPile();
        }

        private void ShuffleStockPile()
        {
            var startCards = currentGameState.CurrentTable.StockPile.Cards.ToArray();
            var shuffled = shuffler.Shuffle(new CardCollection(startCards));
            currentGameState.CurrentTable.StockPile = new StockPile(shuffled);
            currentGameState.Events.Add(new Shuffled(GetNextEventNumber(currentGameState.Events), CardMoveSources.StockPile, new CardCollection(startCards), shuffled));
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
    }
}
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
        private readonly TurnBuilder turnBuilder;

        public TakeCommand(IRules rules, IShuffler shuffler, GameState currentGameState, TakeCommandContext context)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.currentGameState = currentGameState;
            this.context = context;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            var validPlays = GetValidPlays(context.ExecutingPlayer.Hand,
                    currentGameState.CurrentTable.DiscardPile.CardToMatch, currentGameState.CurrentTurn.TurnNumber,
                    null)
                .ToArray();

            if (validPlays.Any())
                return new ActionResult(false, ActionResultMessageKey.InvalidTake);

            if (currentGameState.CurrentTurn.PlayerToPlay.Number != context.ExecutingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var takenCard =
                currentGameState.CurrentTable.MoveCardFromStockPileToPlayer(currentGameState.CurrentTurn.PlayerToPlay);
            currentGameState.Events.Add(new Taken(GetNextEventNumber(currentGameState.Events),
                currentGameState.CurrentTurn.PlayerToPlay.Number, takenCard));

            if (currentGameState.CurrentTable.StockPile.IsEmpty())
                MoveDiscardPileToStockPile();

            var selectedSuit = currentGameState.PreviousTurnResult?.SelectedSuit;
            currentGameState.PreviousTurnResult = new PreviousTurnResult(false, null, selectedSuit, takenCard);
            currentGameState.CurrentTurn = turnBuilder.BuildNextTurn(currentGameState, currentGameState.NextPlayer, selectedSuit);

            return currentGameState;
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
                currentGameState.Events.Add(new CardMoved(GetNextEventNumber(currentGameState.Events), card,
                    CardMoveSources.DiscardPile,
                    CardMoveSources.StockPile));
            }

            ShuffleStockPile();
        }

        private void ShuffleStockPile()
        {
            var startCards = currentGameState.CurrentTable.StockPile.Cards.ToArray();
            var shuffled = shuffler.Shuffle(new CardCollection(startCards));
            currentGameState.CurrentTable.StockPile = new StockPile(shuffled);
            currentGameState.Events.Add(new Shuffled(GetNextEventNumber(currentGameState.Events),
                CardMoveSources.StockPile, new CardCollection(startCards), shuffled));
        }
    }
}
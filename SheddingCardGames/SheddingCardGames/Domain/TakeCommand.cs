using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class TakeCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly Player executingPlayer;
        private readonly IRules rules;
        private readonly IShuffler shuffler;
        private readonly TurnBuilder turnBuilder;

        public TakeCommand(IRules rules, IShuffler shuffler, GameState gameState, Player executingPlayer)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.gameState = gameState;
            this.executingPlayer = executingPlayer;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            var validPlays = rules.GetValidPlays(gameState.CurrentCardToMatch,
                    executingPlayer.Hand,
                    gameState.CurrentTurnNumber,
                    null)
                .ToArray();

            if (validPlays.Any())
                return new ActionResult(false, ActionResultMessageKey.InvalidTake);

            if (gameState.CurrentPlayerToPlayNumber != executingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var takenCard =
                gameState.CurrentTable.MoveCardFromStockPileToPlayer(gameState.CurrentPlayerToPlay);
            gameState.Events.Add(new Taken(gameState.NextEventNumber,
                gameState.CurrentPlayerToPlayNumber, takenCard));

            if (gameState.CurrentTable.StockPile.IsEmpty())
                MoveDiscardPileToStockPile();

            var selectedSuit = gameState.CurrentSelectedSuit;
            gameState.PreviousTurnResult = new PreviousTurnResult(false, null, selectedSuit, takenCard);
            gameState.CurrentTurn =
                turnBuilder.BuildNextTurn(gameState, gameState.NextPlayer, selectedSuit);

            return gameState;
        }

        private void MoveDiscardPileToStockPile()
        {
            var cardsToRemove = gameState.CurrentTable.DiscardPile.RestOfCards.Cards.ToArray();

            foreach (var card in cardsToRemove)
            {
                gameState.CurrentTable.MoveCardFromDiscardPileToStockPile();
                gameState.Events.Add(new CardMoved(gameState.NextEventNumber, card,
                    CardMoveSources.DiscardPile,
                    CardMoveSources.StockPile));
            }

            ShuffleStockPile();
        }

        private void ShuffleStockPile()
        {
            var startCards = gameState.CurrentTable.StockPile.Cards.ToArray();
            var shuffled = shuffler.Shuffle(new CardCollection(startCards));
            gameState.CurrentTable.StockPile = new StockPile(shuffled);
            gameState.Events.Add(new Shuffled(gameState.NextEventNumber,
                CardMoveSources.StockPile, new CardCollection(startCards), shuffled));
        }
    }
}
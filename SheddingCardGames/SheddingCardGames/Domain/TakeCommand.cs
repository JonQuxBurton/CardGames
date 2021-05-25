using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class TakeCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly TakeContext takeContext;
        private readonly IRules rules;
        private readonly IShuffler shuffler;
        private readonly TurnBuilder turnBuilder;

        public TakeCommand(IRules rules, IShuffler shuffler, GameState gameState, TakeContext takeContext)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.gameState = gameState;
            this.takeContext = takeContext;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            var validPlays = rules.HasValidPlay(gameState.CurrentCardToMatch,
                    takeContext.ExecutingPlayer.Hand,
                    gameState.CurrentSelectedSuit, 
                    gameState.AnyPlaysOrTakes);

            if (validPlays)
                return new ActionResult(false, ActionResultMessageKey.InvalidTake);

            if (gameState.CurrentPlayerToPlayNumber != takeContext.ExecutingPlayer.Number)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var takenCard =
                gameState.CurrentTable.MoveCardFromStockPileToPlayer(gameState.CurrentPlayerToPlay);
            gameState.AddEvent(new Taken(gameState.NextEventNumber,
                gameState.CurrentPlayerToPlayNumber, takenCard));

            if (gameState.CurrentTable.StockPile.IsEmpty())
                MoveDiscardPileToStockPile();

            var selectedSuit = gameState.CurrentSelectedSuit;
            gameState.PreviousTurnResult = new PreviousTurnResult(false, null, selectedSuit, takenCard);
            gameState.CurrentTurn =
                turnBuilder.BuildNextTurn(gameState, gameState.CurrentPlayerToPlay, selectedSuit);

            return gameState;
        }

        private void MoveDiscardPileToStockPile()
        {
            var cardsToRemove = gameState.CurrentTable.DiscardPile.RestOfCards.Cards.ToArray();

            foreach (var card in cardsToRemove)
            {
                gameState.CurrentTable.MoveCardFromDiscardPileToStockPile();
                gameState.AddEvent(new CardMoved(gameState.NextEventNumber, card,
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
            gameState.AddEvent(new Shuffled(gameState.NextEventNumber,
                CardMoveSources.StockPile, new CardCollection(startCards), shuffled));
        }
    }
}
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class DealCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly DealContext dealContext;
        private readonly CrazyEightsRules crazyEightsRules;
        private readonly IShuffler shuffler;
        private readonly TurnBuilder turnBuilder;

        public DealCommand(CrazyEightsRules crazyEightsRules, IShuffler shuffler, GameState gameState, DealContext dealContext)
        {
            this.shuffler = shuffler;
            this.crazyEightsRules = crazyEightsRules;
            this.gameState = gameState;
            this.dealContext = dealContext;

            turnBuilder = new TurnBuilder(crazyEightsRules);
        }

        public override ActionResult IsValid()
        {
            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var shuffled = shuffler.Shuffle(dealContext.Deck);
            gameState.CurrentTable = Deal(shuffled);
            gameState.AddEvent(new DealCompleted(gameState.NextEventNumber));
            gameState.CurrentTurn = turnBuilder.BuildFirstTurn(gameState, gameState.PlayerToStart);
            gameState.CurrentGamePhase = GamePhase.InGame;
            
            return gameState;
        }

        private Table Deal(CardCollection cardsToDeal)
        {
            var table = new Table(new StockPile(cardsToDeal), new DiscardPile(), gameState.Players);

            for (var i = 0; i < crazyEightsRules.GetHandSize(); i++)
            {
                if (table.StockPile.IsEmpty()) break;

                for (var j = 0; j < gameState.Players.Count(); j++)
                {
                    var player = table.Players[j];
                    var takenCard = table.MoveCardFromStockPileToPlayer(player);
                    gameState.AddEvent(new CardMoved(gameState.NextEventNumber, 
                        takenCard,
                        CardMoveSources.StockPile, 
                        CardMoveSources.PlayerHand(player.Number)));
                }
            }

            var cardTurnedUp = table.MoveCardFromStockPileToDiscardPile();
            gameState.AddEvent(new CardMoved(gameState.NextEventNumber, 
                cardTurnedUp,
                CardMoveSources.StockPile, 
                CardMoveSources.DiscardPile));

            return table;
        }
    }
}
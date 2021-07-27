using System.Linq;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class DealCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly DealContext dealContext;
        private readonly CrazyEightsRules crazyEightsRules;
        private readonly IShuffler shuffler;
        private readonly CurrentTurnBuilder currentTurnBuilder;

        public DealCommand(CrazyEightsRules crazyEightsRules, IShuffler shuffler, GameState gameState, DealContext dealContext)
        {
            this.shuffler = shuffler;
            this.crazyEightsRules = crazyEightsRules;
            this.gameState = gameState;
            this.dealContext = dealContext;

            currentTurnBuilder = new CurrentTurnBuilder(crazyEightsRules);
        }

        public override IsValidResult IsValid()
        {
            return new IsValidResult(true, CommandIsValidResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var shuffled = shuffler.Shuffle(dealContext.Deck);
            gameState.CurrentTable = Deal(shuffled);
            gameState.EventLog.AddEvent(new DealCompleted(gameState.EventLog.NextEventNumber));
            gameState.CurrentStateOfTurn = currentTurnBuilder.BuildFirstTurn(gameState, gameState.GameSetup.PlayerToStart);
            gameState.CurrentStateOfPlay = StateOfPlay.WithGamePhaseInGame(gameState.CurrentStateOfPlay);
            
            return gameState;
        }

        private Table Deal(CardCollection cardsToDeal)
        {
            var table = new Table(new StockPile(cardsToDeal), new DiscardPile(), gameState.GameSetup.Players);

            for (var i = 0; i < crazyEightsRules.GetHandSize(); i++)
            {
                if (table.StockPile.IsEmpty()) break;

                for (var j = 0; j < gameState.GameSetup.Players.Count(); j++)
                {
                    var player = table.Players[j];
                    var takenCard = table.MoveCardFromStockPileToPlayer(player);
                    gameState.EventLog.AddEvent(new CardMoved(gameState.EventLog.NextEventNumber, 
                        takenCard,
                        CardMoveSources.StockPile, 
                        CardMoveSources.PlayerHand(player.Number)));
                }
            }

            var cardTurnedUp = table.MoveCardFromStockPileToDiscardPile();
            gameState.EventLog.AddEvent(new CardMoved(gameState.EventLog.NextEventNumber, 
                cardTurnedUp,
                CardMoveSources.StockPile, 
                CardMoveSources.DiscardPile));

            return table;
        }
    }
}
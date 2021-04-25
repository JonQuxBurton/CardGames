using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class DealCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly DealContext dealContext;
        private readonly IRules rules;
        private readonly IShuffler shuffler;
        private readonly TurnBuilder turnBuilder;

        public DealCommand(IRules rules, IShuffler shuffler, GameState gameState, DealContext dealContext)
        {
            this.shuffler = shuffler;
            this.rules = rules;
            this.gameState = gameState;
            this.dealContext = dealContext;

            turnBuilder = new TurnBuilder(rules);
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
            var playersArray = dealContext.Players ?? dealContext.Players.ToArray();
            var table = new Table(new StockPile(cardsToDeal), new DiscardPile(), playersArray.ToArray());

            for (var i = 0; i < rules.GetHandSize(); i++)
            {
                if (table.StockPile.IsEmpty()) break;

                for (var j = 0; j < playersArray.Count(); j++)
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
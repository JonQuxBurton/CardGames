using System.Linq;
using SheddingCardGames.Domain.CrazyEights;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain.Whist
{
    public class DealCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly DealContext dealContext;
        private readonly WhistConfiguration whistConfiguration;
        private readonly IShuffler shuffler;
        private readonly CurrentTrickBuilder currentTrickBuilder;

        public DealCommand(WhistConfiguration whistConfiguration, IShuffler shuffler, GameState gameState, DealContext dealContext)
        {
            this.whistConfiguration = whistConfiguration;
            this.shuffler = shuffler;
            this.gameState = gameState;
            this.dealContext = dealContext;

            currentTrickBuilder = new CurrentTrickBuilder();
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
            gameState.CurrentStateOfTrick = currentTrickBuilder.BuildFirstTrick(gameState, gameState.GameSetup.PlayerToStart);
            gameState.CurrentStateOfPlay = StateOfPlay.WithGamePhaseInGame(gameState.CurrentStateOfPlay);
            
            return gameState;
        }

        private Table Deal(CardCollection cardsToDeal)
        {
            var table = new Table(new StockPile(cardsToDeal), gameState.GameSetup.Players);

            for (var i = 0; i < whistConfiguration.HandSize; i++)
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
            
            return table;
        }
    }
}
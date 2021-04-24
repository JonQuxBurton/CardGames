using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class DealCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly CardCollection deck;
        private readonly Player[] players;
        private readonly IRules rules;
        private readonly IShuffler shuffler;
        private readonly TurnBuilder turnBuilder;

        public DealCommand(IShuffler shuffler, IRules rules, GameState gameState, CardCollection deck,
            Player[] players)
        {
            this.shuffler = shuffler;
            this.rules = rules;
            this.gameState = gameState;
            this.deck = deck;
            this.players = players;

            turnBuilder = new TurnBuilder(rules);
        }

        public override ActionResult IsValid()
        {
            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var shuffled = shuffler.Shuffle(deck);
            gameState.CurrentTable = Deal(shuffled, gameState.Events);
            gameState.Events.Add(new DealCompleted(gameState.NextEventNumber));
            gameState.CurrentTurn = turnBuilder.BuildFirstTurn(gameState, gameState.PlayerToStart);

            return gameState;
        }

        private Table Deal(CardCollection cardsToDeal, List<DomainEvent> events)
        {
            var playersArray = players ?? players.ToArray();
            var table = new Table(new StockPile(cardsToDeal), new DiscardPile(), playersArray.ToArray());

            for (var i = 0; i < rules.GetHandSize(); i++)
            {
                if (table.StockPile.IsEmpty()) break;

                for (var j = 0; j < playersArray.Count(); j++)
                {
                    var player = table.Players[j];
                    var takenCard = table.MoveCardFromStockPileToPlayer(player);
                    events.Add(new CardMoved(events.Select(x => x.Number).DefaultIfEmpty().Max() + 1, takenCard,
                        CardMoveSources.StockPile, CardMoveSources.PlayerHand(player.Number)));
                }
            }

            var cardTurnedUp = table.MoveCardFromStockPileToDiscardPile();
            events.Add(new CardMoved(events.Select(x => x.Number).DefaultIfEmpty().Max() + 1, cardTurnedUp,
                CardMoveSources.StockPile, CardMoveSources.DiscardPile));

            return table;
        }
    }
}
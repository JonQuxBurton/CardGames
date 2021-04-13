using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain
{
    public class Dealer : IDealer
    {
        private readonly IRules rules;

        public Dealer(IRules rules)
        {
            this.rules = rules;
        }

        public Table Deal(IEnumerable<Player> players, CardCollection cardsToDeal, List<DomainEvent> events)
        {
            var playersArray = players as Player[] ?? players.ToArray();
            var table = new Table(new StockPile(cardsToDeal), new DiscardPile(), playersArray.ToArray());

            for (var i = 0; i < rules.GetHandSize(); i++)
            {
                if (table.StockPile.IsEmpty()) break;

                for (var j = 0; j < playersArray.Count(); j++)
                {
                    var player = table.Players[j];
                    var takenCard = table.MoveCardFromStockPileToPlayer(player);
                    events.Add(new CardMoved(events.Select(x => x.Number).DefaultIfEmpty().Max() + 1, takenCard,
                        CardMoveSources.StockPile, GetPlayerSource(player)));
                }
            }

            var cardTurnedUp = table.MoveCardFromStockPileToDiscardPile();
            events.Add(new CardMoved(events.Select(x => x.Number).DefaultIfEmpty().Max() + 1, cardTurnedUp,
                CardMoveSources.StockPile, CardMoveSources.DiscardPile));

            return table;
        }

        private static string GetPlayerSource(Player player)
        {
            return CardMoveSources.PlayerHand(player.Number);
        }
    }
}
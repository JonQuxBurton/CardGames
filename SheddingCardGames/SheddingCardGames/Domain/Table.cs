using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class Table
    {
        public Table(StockPile stockPile, DiscardPile discardPile, params Player[] players)
        {
            Players = new ReadOnlyCollection<Player>(players);
            StockPile = stockPile;
            DiscardPile = discardPile;
        }

        public ReadOnlyCollection<Player> Players { get; }
        public StockPile StockPile { get; set; }
        public DiscardPile DiscardPile { get; set; }

        public CardCollection AllCards
        {
            get
            {
                var playersCard = new List<Card>();
                foreach (var player in Players) playersCard.AddRange(player.Hand.Cards);

                return new CardCollection(
                    new List<Card>()
                        .Concat(playersCard)
                        .Concat(DiscardPile.AllCards.Cards)
                        .Concat(StockPile.Cards));
            }
        }

        public Card MoveCardFromStockPileToDiscardPile()
        {
            var cardToTurnUp = StockPile.Take();
            DiscardPile.AddCard(cardToTurnUp);
            return cardToTurnUp;
        }

        public void MoveCardFromPlayerToDiscardPile(Player player, Card card)
        {
            player.Hand.Remove(card);
            DiscardPile.AddCard(card);
        }

        public void MoveCardsFromPlayerToDiscardPile(Player player, IImmutableList<Card> cards)
        {
            foreach (var card in cards) 
                MoveCardFromPlayerToDiscardPile(player, card);
        }

        public Card MoveCardFromStockPileToPlayer(Player player)
        {
            var takenCard = StockPile.Take();
            player.Hand.AddAtEnd(takenCard);
            return takenCard;
        }

        public Card MoveCardFromDiscardPileToStockPile()
        {
            var card = DiscardPile.RestOfCards.TakeFromStart();
            StockPile.AddAtEnd(card);
            return card;
        }
    }
}
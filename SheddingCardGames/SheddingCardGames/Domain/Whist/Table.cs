using System.Collections.Immutable;

namespace SheddingCardGames.Domain.Whist
{
    public class Table
    {
        public Table(StockPile stockPile, IImmutableList<Player> players, Trick trick = null)
        {
            Players = players;
            StockPile = stockPile;

            Trick = trick ?? new Trick(new CardCollection());
        }

        public IImmutableList<Player> Players { get; }
        public StockPile StockPile { get; set; }
        public Trick Trick { get; }


        public void MoveCardFromPlayerToTrick(Player player, Card card)
        {
            player.Hand.Remove(card);
            Trick.AddCard(card);
        }

        public Card MoveCardFromStockPileToPlayer(Player player)
        {
            var takenCard = StockPile.Take();
            player.Hand.AddAtEnd(takenCard);
            return takenCard;
        }
    }
}
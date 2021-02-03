using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class Board
    {
        private readonly List<CardMoveEvent> cardMoves;

        public Board(Player player1, Player player2, StockPile stockPile, DiscardPile discardPile)
        {
            Player1 = player1;
            Player2 = player2;
            StockPile = stockPile;
            DiscardPile = discardPile;
            cardMoves = new List<CardMoveEvent>();
        }

        public Player Player1 { get; }
        public Player Player2 { get; }
        public StockPile StockPile { get; }
        public DiscardPile DiscardPile { get; set; }
        public IEnumerable<CardMoveEvent> CardMoves => cardMoves;

        public CardCollection AllCards =>
            new CardCollection(
                new List<Card>().Concat(Player1.Hand.Cards).Concat(Player2.Hand.Cards)
                    .Concat(DiscardPile.AllCards.Cards).Concat(StockPile.Cards));

        public void TurnUpDiscardCard()
        {
            var cardToTurnUp = StockPile.Take();
            DiscardPile.AddCard(cardToTurnUp);
            cardMoves.Add(new CardMoveEvent(cardToTurnUp, CardMoveSources.StockPile, CardMoveSources.DiscardPile));
        }

        public void MoveCardToDiscardPile(Player player, Card card)
        {
            player.Hand.Remove(card);
            DiscardPile.AddCard(card);
            cardMoves.Add(new CardMoveEvent(card, GetPlayerSource(player), CardMoveSources.DiscardPile));
        }

        public Card TakeCardFromStockPile(Player player)
        {
            var takenCard = StockPile.Take();
            player.Hand.AddAtEnd(takenCard);
            cardMoves.Add(new CardMoveEvent(takenCard, CardMoveSources.StockPile, GetPlayerSource(player)));
            return takenCard;
        }

        public void MoveDiscardPileToStockPile()
        {
            var cardsToRemove = DiscardPile.TakeRestOfCards();

            foreach (var card in cardsToRemove.Cards)
            {
                StockPile.AddAtEnd(card);
                cardMoves.Add(new CardMoveEvent(card, CardMoveSources.DiscardPile, CardMoveSources.StockPile));
            }
        }

        private static string GetPlayerSource(Player player)
        {
            var playerSource = CardMoveSources.Player1Hand;
            if (player.Number == 2)
                playerSource = CardMoveSources.Player2Hand;
            return playerSource;
        }
    }
}
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class Board
    {
        private readonly List<CardMoveEvent> cardMoves;

        public Board(StockPile stockPile, DiscardPile discardPile, params Player[] players)
        {
            Players = new ReadOnlyCollection<Player>(players);
            StockPile = stockPile;
            DiscardPile = discardPile;
            cardMoves = new List<CardMoveEvent>();
        }

        public ReadOnlyCollection<Player> Players { get; }
        public StockPile StockPile { get; }
        public DiscardPile DiscardPile { get; set; }
        public IEnumerable<CardMoveEvent> CardMoves => cardMoves;

        public CardCollection AllCards
        {
            get
            {
                var playersCard = new List<Card>();
                foreach (var player in Players)
                {
                    playersCard.AddRange(player.Hand.Cards);
                }

                return new CardCollection(
                    new List<Card>()
                        .Concat(playersCard)
                        .Concat(DiscardPile.AllCards.Cards)
                        .Concat(StockPile.Cards));
            }
        }

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
            return CardMoveSources.PlayerHand(player.Number);
        }
    }
}
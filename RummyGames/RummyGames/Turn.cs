using System;
using CardGamesDomain;

namespace RummyGames
{
    public class Turn
    {
        public Turn(int number, Guid currentPlayerId, Card cardTakenFromStockPile = null, Card cardTakenFromDiscardPile = null)
        {
            Number = number;
            CurrentPlayerId = currentPlayerId;
            CardTakenFromStockPile = cardTakenFromStockPile;
            CardTakenFromDiscardPile = cardTakenFromDiscardPile;
        }

        public int Number { get; }
        public Guid CurrentPlayerId { get; }
        public Card CardTakenFromStockPile { get; }
        public Card CardTakenFromDiscardPile { get; }

        public bool HasTakenCard() => CardTakenFromStockPile != null || CardTakenFromDiscardPile != null;
    }
}
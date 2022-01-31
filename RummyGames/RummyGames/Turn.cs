using System;
using CardGamesDomain;

namespace RummyGames
{
    public class Turn
    {
        public Turn(int number, Guid currentPlayerId, Card takenCard = null)
        {
            Number = number;
            CurrentPlayerId = currentPlayerId;
            TakenCard = takenCard;
        }

        public int Number { get; }
        public Guid CurrentPlayerId { get; }
        public Card TakenCard { get; }
    }
}
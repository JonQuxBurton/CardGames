using System;
using CardGamesDomain;

namespace RummyGames
{
    public class Player
    {
        public Player(Guid id, string name)
        {
            Id = id;
            Name = name;
            Hand = new Hand();
        }
        
        public Player(Guid id, string name, Hand hand)
        {
            Id = id;
            Name = name;
            Hand = hand;
        }

        public Guid Id { get; }
        public string Name { get; }
        public Hand Hand { get; }
    }
}
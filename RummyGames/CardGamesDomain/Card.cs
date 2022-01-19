using System;

namespace CardGamesDomain
{
    public class Card : IEquatable<Card>
    {
        public Card()
        {
        }

        public Card(Rank rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public Rank Rank { get; set; }
        public Suit Suit { get; set; }

        public bool Equals(Card other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Rank == other.Rank && Suit == other.Suit;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Card) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int) Rank, (int) Suit);
        }

        public override string ToString()
        {
            return $"{Rank} {Suit}";
        }
    }
}
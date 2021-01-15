using System;
using System.Diagnostics.CodeAnalysis;

namespace SheddingCardGames
{
    public class Card : IEquatable<Card>
    {
        public Card(int rank, Suit suit)
        {
            Rank = rank;
            Suit = suit;
        }

        public int Rank { get; }
        public Suit Suit { get; }

        [ExcludeFromCodeCoverage]
        public bool Equals(Card other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Rank == other.Rank && Suit == other.Suit;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Card) obj);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            unchecked
            {
                return (Rank * 397) ^ (int) Suit;
            }
        }

        public override string ToString()
        {
            return $"{Rank} {Suit}";
        }
    }
}
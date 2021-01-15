using System;
using System.Diagnostics.CodeAnalysis;

namespace SheddingCardGames
{
    public class CardMoveEvent : IEquatable<CardMoveEvent>
    {
        public Card Card { get; }
        public string FromSource { get; }
        public string ToSource { get; }

        public CardMoveEvent(Card card, string fromSource, string toSource)
        {
            Card = card;
            FromSource = fromSource;
            ToSource = toSource;
        }

        [ExcludeFromCodeCoverage]
        public bool Equals(CardMoveEvent other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Card, other.Card) && FromSource == other.FromSource && ToSource == other.ToSource;
        }

        [ExcludeFromCodeCoverage]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CardMoveEvent) obj);
        }

        [ExcludeFromCodeCoverage]
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Card != null ? Card.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (FromSource != null ? FromSource.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (ToSource != null ? ToSource.GetHashCode() : 0);
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"[{Card} from {FromSource} to {ToSource}]";
        }
    }
}
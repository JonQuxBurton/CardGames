using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public static class CardsUtils
    {
        public static ImmutableList<Card> Cards(params Card[] cards)
        {
            return ImmutableList.Create(cards);
        }

        public static Card Card(int rank, Suit suit)
        {
            return new Card(rank, suit);
        }
    }
}
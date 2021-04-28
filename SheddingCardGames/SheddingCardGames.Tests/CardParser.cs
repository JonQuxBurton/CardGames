using System;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests
{
    public class CardParser
    {
        public Card Parse(string cardAsString)
        {
            var split = cardAsString.Split('|');
            var rank = int.Parse(split[0]);
            var suit = (Suit) Enum.Parse(typeof(Suit), split[1]);

            return new Card(rank, suit);
        }
    }
}
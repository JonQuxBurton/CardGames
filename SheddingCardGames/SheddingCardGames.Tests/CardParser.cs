using System;
using System.Linq;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests
{
    public class CardParser
    {
        public Card[] Parse(string[] cardsAsString)
        {
            return cardsAsString.Select(Parse).ToArray();
        }

        public Card Parse(string cardAsString)
        {
            var split = cardAsString.Split('|');
            var rank = int.Parse(split[0]);
            var suit = (Suit) Enum.Parse(typeof(Suit), split[1]);

            return new Card(rank, suit);
        }
    }
}
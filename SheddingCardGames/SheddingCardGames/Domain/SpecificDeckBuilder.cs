using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class SpecificDeckBuilder : IDeckBuilder
    {
        public SpecificDeckBuilder(CardCollection player1Hand, CardCollection player2Hand, Card discardCard,
            CardCollection stockPile)
        {
            Player1Hand = player1Hand;
            Player2Hand = player2Hand;
            DiscardCard = discardCard;
            StockPile = stockPile;
        }

        public CardCollection Player1Hand { get; }

        public CardCollection Player2Hand { get; }

        public Card DiscardCard { get; }

        public CardCollection StockPile { get; }

        public CardCollection Build()
        {
            var cards = new List<Card>();
            for (var i = 0; i < Player1Hand.Cards.Count(); i++)
            {
                cards.Add(Player1Hand.Cards.ElementAt(i));
                cards.Add(Player2Hand.Cards.ElementAt(i));
            }

            cards.Add(DiscardCard);
            cards.AddRange(StockPile.Cards);

            return new CardCollection(cards);
        }
    }
}
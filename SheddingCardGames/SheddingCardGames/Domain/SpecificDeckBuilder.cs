using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class SpecificDeckBuilder : IDeckBuilder
    {
        public SpecificDeckBuilder(CardCollection[] playerHands,
            Card discardCard,
            CardCollection stockPile)
        {
            Player1Hand = playerHands[0];
            Player2Hand = playerHands[1];

            if (playerHands.Length > 2)
                Player3Hand = playerHands[2];

            DiscardPile = new CardCollection(discardCard);
            StockPile = stockPile;
        }
        
        public SpecificDeckBuilder(CardCollection[] playerHands,
            CardCollection discardPile,
            CardCollection stockPile)
        {
            Player1Hand = playerHands[0];
            Player2Hand = playerHands[1];

            if (playerHands.Length > 2)
                Player3Hand = playerHands[2];

            DiscardPile = discardPile;
            StockPile = stockPile;
        }

        private CardCollection Player1Hand { get; }

        private CardCollection Player2Hand { get; }
        private CardCollection Player3Hand { get; }

        private CardCollection DiscardPile { get; }

        private CardCollection StockPile { get; }

        public CardCollection Build()
        {
            var cards = new List<Card>();
            for (var i = 0; i < Player1Hand.Cards.Count(); i++)
            {
                cards.Add(Player1Hand.Cards.ElementAt(i));
                cards.Add(Player2Hand.Cards.ElementAt(i));

                if (Player3Hand != null && Player3Hand.Count() > i)
                    cards.Add(Player3Hand.Cards.ElementAt(i));
            }

            cards.AddRange(DiscardPile.Cards);
            cards.AddRange(StockPile.Cards);

            return new CardCollection(cards);
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class SpecificDeckBuilder : IDeckBuilder
    {
        public SpecificDeckBuilder(Card discardCard, CardCollection stockPile, params CardCollection[] playerHands)
        {
            DiscardPile = new CardCollection(discardCard);
            StockPile = stockPile;
            PlayerHands = playerHands;
        }

        public SpecificDeckBuilder(CardCollection discardPile, CardCollection stockPile,
            params CardCollection[] playerHands)
        {
            DiscardPile = discardPile;
            StockPile = stockPile;
            PlayerHands = playerHands;
        }

        private CardCollection[] PlayerHands { get; }

        private CardCollection DiscardPile { get; }

        private CardCollection StockPile { get; }

        public CardCollection Build()
        {
            var cards = new List<Card>();

            for (var i = 0; i < PlayerHands[0].Cards.Count(); i++)
                foreach (var playerHand in PlayerHands)
                    cards.Add(playerHand.Cards.ElementAt(i));

            cards.AddRange(DiscardPile.Cards);
            cards.AddRange(StockPile.Cards);

            return new CardCollection(cards);
        }
    }
}
using System.Collections.Generic;

namespace SheddingCardGames.Domain
{
    public class StockPile
    {
        private readonly CardCollection cards;

        public StockPile(CardCollection cards)
        {
            this.cards = cards;
        }

        public IEnumerable<Card> Cards => cards.Cards;

        public Card Take()
        {
            return cards.TakeFromStart();
        }

        public bool IsEmpty()
        {
            return cards.IsEmpty();
        }

        public void AddAtEnd(Card card)
        {
            cards.AddAtEnd(card);
        }
    }
}
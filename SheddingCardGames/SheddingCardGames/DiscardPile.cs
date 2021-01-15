using System.Collections.Generic;

namespace SheddingCardGames
{
    public class DiscardPile
    {
        public DiscardPile()
        {
            this.RestOfCards = new CardCollection();
        }
        
        public DiscardPile(IEnumerable<Card> restOfCards)
        {
            this.RestOfCards = new CardCollection(restOfCards);
        }

        public CardCollection RestOfCards { get; }

        public Card CardToMatch { get; private set; }

        public void TurnUpTopCard()
        {
            CardToMatch = RestOfCards.TakeFromStart();
        }

        public void AddCard(Card card)
        {
            if (CardToMatch != null)
                RestOfCards.AddAtStart(CardToMatch);
            CardToMatch = card;
        }

        public CardCollection TakeRestOfCards()
        {
            var cards = new List<Card>();
            while (!RestOfCards.IsEmpty())
                cards.Add(RestOfCards.TakeFromStart());

            return new CardCollection(cards);
        }
    }
}
using System.Collections.Generic;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class DiscardPile
    {
        public DiscardPile()
        {
            RestOfCards = new CardCollection();
        }

        public DiscardPile(IEnumerable<Card> restOfCards)
        {
            RestOfCards = new CardCollection(restOfCards);
        }

        public DiscardPile(CardCollection restOfCards)
        {
            RestOfCards = restOfCards;
        }

        public CardCollection AllCards
        {
            get
            {
                var cards = new List<Card>();

                if (CardToMatch != null)
                    cards.Add(CardToMatch);

                cards.AddRange(RestOfCards.Cards);
                return new CardCollection(cards);
            }
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
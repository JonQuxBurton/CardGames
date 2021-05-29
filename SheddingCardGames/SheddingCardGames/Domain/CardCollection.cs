using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class CardCollection
    {
        private readonly List<Card> cards;

        public IEnumerator<Card> GetEnumerator => cards.GetEnumerator();

        public CardCollection()
        {
            cards = new List<Card>();
        }

        public CardCollection(params Card[] cards)
        {
            this.cards = cards.ToList();
        }
        
        public CardCollection(IEnumerable<Card> cards)
        {
            this.cards = cards.ToList();
        }

        public IEnumerable<Card> Cards => cards;

        public Card Get(int rank, Suit suit)
        {
            return cards.FirstOrDefault(x => x.Rank == rank && x.Suit == suit);
        }

        public CardCollection GetCardCollection(params Card[] cardsToGet)
        {
            var matchingCards = new List<Card>();
            foreach (var cardToGet in cardsToGet)
            {
                var card = cards.FirstOrDefault(x => x.Rank == cardToGet.Rank && x.Suit == cardToGet.Suit);
                if (card != null)
                    matchingCards.Add(card);
            }

            return new CardCollection(matchingCards);
        }

        public void AddAtStart(Card card)
        {
            cards.Insert(0, card);
        }

        public void AddAtEnd(Card card)
        {
            cards.Add(card);
        }

        public Card TakeFromStart()
        {
            var first = cards.FirstOrDefault();
            
            if (cards.Any())
                cards.RemoveAt(0);
            
            return first;
        }
        
        public void Remove(Card card)
        {
            cards.Remove(card);
        }

        public bool Contains(Card card)
        {
            return cards.Contains(card);
        }
        
        public bool ContainsAll(IImmutableList<Card> cardsToCheck)
        {
            if (cardsToCheck == null || !cardsToCheck.Any())
                return false;

            return !cardsToCheck.Except(cards).Any();
        }

        public bool IsEmpty()
        {
            return !cards.Any();
        }

        public Card First()
        {
            return cards.First();
        }

        public int Count()
        {
            return cards.Count();
        }
    }
}
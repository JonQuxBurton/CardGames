using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain.Whist
{
    public class SpecificDeckBuilder : IDeckBuilder
    {
        public SpecificDeckBuilder(params CardCollection[] playerHands)
        {
            PlayerHands = playerHands;
        }
        
        private CardCollection[] PlayerHands { get; }

        public CardCollection Build()
        {
            var cards = new List<Card>();

            for (var i = 0; i < PlayerHands[0].Cards.Count(); i++)
                foreach (var playerHand in PlayerHands)
                    cards.Add(playerHand.Cards.ElementAt(i));
            
            return new CardCollection(cards);
        }
    }
}
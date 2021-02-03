using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class DummyShuffler : IShuffler
    {
        public CardCollection Shuffle(CardCollection cards)
        {
            return cards;
        }
    }
}
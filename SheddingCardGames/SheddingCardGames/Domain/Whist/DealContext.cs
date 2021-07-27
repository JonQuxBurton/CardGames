namespace SheddingCardGames.Domain.Whist
{
    public class DealContext : ICommandContext
    {
        public DealContext(CardCollection deck)
        {
            Deck = deck;
        }

        public CardCollection Deck { get; }
    }
}
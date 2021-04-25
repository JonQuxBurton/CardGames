namespace SheddingCardGames.Domain
{
    public class DealContext : ICommandContext
    {
        public DealContext(CardCollection deck, Player[] players)
        {
            Deck = deck;
            Players = players;
        }

        public CardCollection Deck { get; }
        public Player[] Players { get; }
    }
}
namespace SheddingCardGames.Domain.Whist
{
    public class Trick
    {
        public Trick(CardCollection cardCollection)
        {
            CardCollection = cardCollection;
        }

        public CardCollection CardCollection { get; }

        public void AddCard(Card card)
        {
            CardCollection.AddAtEnd(card);
        }

        public bool IsCompleted => CardCollection.Count() == 4;
    }
}
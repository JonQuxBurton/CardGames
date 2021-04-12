namespace SheddingCardGames.Domain.Events
{
    public class Shuffled : DomainEvent
    {
        public CardCollection StartCards { get; }
        public CardCollection EndCards { get; }
        public string Target{ get; }
        

        public Shuffled(int number, string target, CardCollection startCards, CardCollection endCards) : base(number)
        {
            Target = target;
            StartCards = startCards;
            EndCards = endCards;
        }
    }
}
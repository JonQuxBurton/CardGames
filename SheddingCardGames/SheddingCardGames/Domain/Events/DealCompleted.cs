namespace SheddingCardGames.Domain.Events
{
    public class DealCompleted : DomainEvent
    {
        public DealCompleted(int number) : base(number)
        {
            
        }
    }
}
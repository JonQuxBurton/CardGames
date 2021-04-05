namespace SheddingCardGames.Domain.Events
{
    public class DomainEvent
    {
        public DomainEvent(int number)
        {
            Number = number;
        }

        public int Number { get; }
    }
}
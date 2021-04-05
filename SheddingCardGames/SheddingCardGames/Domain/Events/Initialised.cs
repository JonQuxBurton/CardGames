namespace SheddingCardGames.Domain.Events
{
    public class Initialised : DomainEvent
    {
        public Initialised(int number) : base(number)
        {
            
        }
    }
}
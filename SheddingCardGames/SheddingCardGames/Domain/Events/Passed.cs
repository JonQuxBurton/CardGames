namespace SheddingCardGames.Domain.Events
{
    public class Passed : DomainEvent
    {
        public int PlayerNumber { get; }
        
        public Passed(int number, int playerNumber) : base(number)
        {
            PlayerNumber = playerNumber;
        }
    }
}
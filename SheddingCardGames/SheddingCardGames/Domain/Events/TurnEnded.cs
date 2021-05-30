namespace SheddingCardGames.Domain.Events
{
    public class TurnEnded : DomainEvent
    {
        public int PlayerNumber { get; }
        
        public TurnEnded(int number, int playerNumber) : base(number)
        {
            PlayerNumber = playerNumber;
        }
    }
}
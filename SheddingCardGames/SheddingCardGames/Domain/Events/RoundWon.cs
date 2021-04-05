namespace SheddingCardGames.Domain.Events
{
    public class RoundWon : DomainEvent
    {
        public int PlayerNumber { get; }

        public RoundWon(int number, int playerNumber) : base(number)
        {
            PlayerNumber = playerNumber;
        }
    }
}
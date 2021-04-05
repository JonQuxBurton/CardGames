namespace SheddingCardGames.Domain.Events
{
    public class SuitSelected : DomainEvent
    {
        public int PlayerNumber { get; }
        public Suit Suit { get; }


        public SuitSelected(int number, int playerNumber, Suit suit) : base(number)
        {
            PlayerNumber = playerNumber;
            Suit = suit;
        }
    }
}
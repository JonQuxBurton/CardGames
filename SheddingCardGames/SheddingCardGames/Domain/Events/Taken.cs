namespace SheddingCardGames.Domain.Events
{
    public class Taken : DomainEvent
    {
        public int PlayerNumber { get; }
        public Card Card { get; }

        public Taken(int number, int playerNumber, Card card) : base(number)
        {
            PlayerNumber = playerNumber;
            Card = card;
        }
    }
}
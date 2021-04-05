namespace SheddingCardGames.Domain.Events
{
    public class Played : DomainEvent
    {
        public int PlayerNumber { get; }
        public Card Card { get; }

        public Played(int number, int playerNumber, Card card) : base(number)
        {
            PlayerNumber = playerNumber;
            Card = card;
        }
    }
}
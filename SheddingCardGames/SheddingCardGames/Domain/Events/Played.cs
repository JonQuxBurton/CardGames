namespace SheddingCardGames.Domain.Events
{
    public class Played : DomainEvent
    {
        public Played(int number, int playerNumber, Card[] cards) : base(number)
        {
            PlayerNumber = playerNumber;
            Cards = cards;
        }

        public int PlayerNumber { get; }
        public Card[] Cards { get; }
    }
}
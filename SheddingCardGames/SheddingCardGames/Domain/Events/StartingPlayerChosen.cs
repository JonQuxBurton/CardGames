namespace SheddingCardGames.Domain.Events
{
    public class StartingPlayerChosen : DomainEvent
    {
        public int PlayerNumber { get; }

        public StartingPlayerChosen(int number, int playerNumber) : base(number)
        {
            PlayerNumber = playerNumber;
        }
    }
}
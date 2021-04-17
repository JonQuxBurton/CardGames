namespace SheddingCardGames.Domain.Events
{
    public class StartingPlayerChosen : DomainEvent
    {
        public Player Player { get; }

        public StartingPlayerChosen(int number, Player player) : base(number)
        {
            Player = player;
        }
    }
}
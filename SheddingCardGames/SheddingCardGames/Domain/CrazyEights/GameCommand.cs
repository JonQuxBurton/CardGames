namespace SheddingCardGames.Domain.CrazyEights
{
    public abstract class GameCommand
    {
        public abstract IsValidResult IsValid();
        public abstract GameState Execute();
    }
}
namespace SheddingCardGames.Domain.Whist
{
    public abstract class GameCommand
    {
        public abstract IsValidResult IsValid();
        public abstract GameState Execute();
    }
}
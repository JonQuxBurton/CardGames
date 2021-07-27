namespace SheddingCardGames.Domain.Whist
{
    public interface ICommandFactory
    {
        GameCommand Create(GameState gameState, ICommandContext context);
    }
}
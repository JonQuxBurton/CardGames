namespace SheddingCardGames.Domain.CrazyEights
{
    public interface ICommandFactory
    {
        GameCommand Create(GameState gameState, ICommandContext context);
    }
}
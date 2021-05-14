using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public interface ICommandFactory
    {
        GameCommand Create(GameState gameState, ICommandContext context);
    }
}
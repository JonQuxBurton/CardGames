using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public abstract class GameCommand
    {
        public abstract IsValidResult IsValid();
        public abstract GameState Execute();
    }
}
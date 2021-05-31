using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public abstract class GameCommand
    {
        public abstract ActionResult IsValid();
        public abstract GameState Execute();
    }
}
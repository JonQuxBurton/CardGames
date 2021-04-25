using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class ChooseStartingPlayerCommand : GameCommand
    {
        private readonly ChooseStartingPlayerContext chooseStartingPlayerContext;

        public ChooseStartingPlayerCommand(ChooseStartingPlayerContext chooseStartingPlayerContext)
        {
            this.chooseStartingPlayerContext = chooseStartingPlayerContext;
        }

        public override ActionResult IsValid()
        {
            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var gameState = new GameState
            {
                PlayerToStart = chooseStartingPlayerContext.ChosenPlayer
            };
            gameState.AddEvent(new StartingPlayerChosen(1, chooseStartingPlayerContext.ChosenPlayer));
            gameState.CurrentGamePhase = GamePhase.ReadyToDeal;

            return gameState;
        }
    }
}
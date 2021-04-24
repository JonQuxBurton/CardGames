using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class ChooseStartingPlayerCommand : GameCommand
    {
        private readonly Player chosenPlayer;

        public ChooseStartingPlayerCommand(Player chosenPlayer)
        {
            this.chosenPlayer = chosenPlayer;
        }

        public override ActionResult IsValid()
        {
            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var currentGameState = new GameState
            {
                PlayerToStart = chosenPlayer
            };
            currentGameState.AddEvent(new StartingPlayerChosen(1, chosenPlayer));

            return currentGameState;
        }
    }
}
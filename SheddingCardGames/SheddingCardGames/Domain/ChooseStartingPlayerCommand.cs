using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class ChooseStartingPlayerCommand : GameCommand
    {
        private readonly GameState gameState;
        private readonly ChooseStartingPlayerContext chooseStartingPlayerContext;

        public ChooseStartingPlayerCommand(GameState gameState, ChooseStartingPlayerContext chooseStartingPlayerContext)
        {
            this.gameState = gameState;
            this.chooseStartingPlayerContext = chooseStartingPlayerContext;
        }

        public override IsValidResult IsValid()
        {
            return new IsValidResult(true, CommandExecutionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            gameState.PlayerToStart = chooseStartingPlayerContext.ChosenPlayer;
            gameState.CurrentGamePhase = GamePhase.ReadyToDeal;
            gameState.AddEvent(new StartingPlayerChosen(1, chooseStartingPlayerContext.ChosenPlayer));

            return gameState;
        }
    }
}
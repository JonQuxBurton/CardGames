using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class ChooseStartingPlayerCommand : GameCommand
    {
        private readonly IRandomPlayerChooser randomPlayerChooser;
        private readonly GameState gameState;
        private readonly ChooseStartingPlayerContext chooseStartingPlayerContext;

        public ChooseStartingPlayerCommand(IRandomPlayerChooser randomPlayerChooser, GameState gameState, ChooseStartingPlayerContext chooseStartingPlayerContext)
        {
            this.randomPlayerChooser = randomPlayerChooser;
            this.gameState = gameState;
            this.chooseStartingPlayerContext = chooseStartingPlayerContext;
        }

        public override IsValidResult IsValid()
        {
            return new IsValidResult(true, CommandExecutionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var chosenPlayer = randomPlayerChooser.ChoosePlayer(gameState.Players);
            gameState.PlayerToStart = chosenPlayer;
            gameState.CurrentGamePhase = GamePhase.ReadyToDeal;
            gameState.AddEvent(new StartingPlayerChosen(1, chosenPlayer));

            return gameState;
        }
    }
}
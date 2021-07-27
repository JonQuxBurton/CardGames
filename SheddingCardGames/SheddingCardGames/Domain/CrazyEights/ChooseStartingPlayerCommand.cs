using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class ChooseStartingPlayerCommand : GameCommand
    {
        private readonly ChooseStartingPlayerContext chooseStartingPlayerContext;
        private readonly GameState gameState;
        private readonly IRandomPlayerChooser randomPlayerChooser;

        public ChooseStartingPlayerCommand(IRandomPlayerChooser randomPlayerChooser, GameState gameState,
            ChooseStartingPlayerContext chooseStartingPlayerContext)
        {
            this.randomPlayerChooser = randomPlayerChooser;
            this.gameState = gameState;
            this.chooseStartingPlayerContext = chooseStartingPlayerContext;
        }

        public override IsValidResult IsValid()
        {
            return new IsValidResult(true, CommandIsValidResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var chosenPlayer = randomPlayerChooser.ChoosePlayer(gameState.GameSetup.Players);
            gameState.GameSetup.WithStartingPlayer(chosenPlayer);
            gameState.CurrentStateOfPlay = StateOfPlay.WithGamePhaseReadyToDeal(gameState.CurrentStateOfPlay);
            gameState.EventLog.AddEvent(new StartingPlayerChosen(1, chosenPlayer));

            return gameState;
        }
    }
}
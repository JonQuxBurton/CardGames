using System.Drawing;

namespace SheddingCardGame.UI
{
    public class NewUiBuilder
    {
        private readonly GameController gameController;
        private readonly int screenWidth;
        private readonly int screenHeight;

        public NewUiBuilder(GameController gameController, int screenWidth, int screenHeight)
        {
            this.gameController = gameController;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }
        
        public UiState BuildNewUiState()
        {
            var newUiState = new UiState();
            newUiState.DealButton = new ButtonComponent("Deal", new Point(screenWidth / 2, screenHeight / 2), true, () => gameController.Deal());
            newUiState.GameObjects.Add(newUiState.DealButton);
            return newUiState;
        }
    }
}

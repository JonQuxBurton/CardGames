namespace SheddingCardGame.UI
{
    public class NewUiBuilder
    {
        private readonly BlazorGameController gameController;
        private readonly int screenWidth;
        private readonly int screenHeight;

        public NewUiBuilder(BlazorGameController gameController, int screenWidth, int screenHeight)
        {
            this.gameController = gameController;
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }
        
        public UiState BuildNewUiState()
        {
            var newUiState = new UiState(null);

            var cursor = new Cursor(screenWidth / 2, screenHeight / 2, 0);
            var buttonBuilder = new ButtonBuilder(newUiState);
            buttonBuilder.Build(cursor, ButtonNames.Deal, ButtonNames.Deal.ToString().ToUpper(), () => gameController.Deal());
            
            return newUiState;
        }
    }
}

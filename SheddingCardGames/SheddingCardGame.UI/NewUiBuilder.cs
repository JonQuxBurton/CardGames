namespace SheddingCardGame.UI
{
    public class NewUiBuilder
    {
        private readonly Config config;
        private readonly BlazorGameController gameController;

        public NewUiBuilder(Config config, BlazorGameController gameController)
        {
            this.config = config;
            this.gameController = gameController;
        }

        public UiState BuildNewUiState()
        {
            var newUiState = new UiState(null);

            var cursor = new Cursor(config.ScreenCentre);
            var buttonBuilder = new ButtonComponentBuilder(config, newUiState);
            buttonBuilder.Build(cursor, ButtonNames.Deal, ButtonNames.Deal.ToString().ToUpper(),
                () => gameController.Deal());

            return newUiState;
        }
    }
}
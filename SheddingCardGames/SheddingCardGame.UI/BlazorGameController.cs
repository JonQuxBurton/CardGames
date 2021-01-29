using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGame.UI
{
    public class BlazorGameController
    {
        private readonly IGameController gameController;
        private readonly InGameUiBuilder inGameUiBuilder;
        public UiState UiState { get; set; }
        public Turn CurrentTurn => gameController.CurrentTurn;

        public GameState GameState => gameController.GameState;

        public BlazorGameController(IGameController gameController, InGameUiBuilder inGameUiBuilder)
        {
            this.gameController = gameController;
            this.inGameUiBuilder = inGameUiBuilder;
        }

        public async void Deal()
        {
            UiState = await inGameUiBuilder.Build(this);
            gameController.Deal();
        }

        public void SelectSuit(Suit suit)
        {
            gameController.SelectSuit(suit);
        }

        public bool Play(CardComponent cardComponent)
        {
            if (!cardComponent.IsTurnedUp)
                return false;

            var isSuccess = gameController.Play(cardComponent.Card);

            if (isSuccess)
                BringToTop(cardComponent);

            return isSuccess;
        }

        public ActionResultWithCard Take()
        {
            var actionResult = gameController.Take();

            if (actionResult.IsSuccess)
            {
                var cardComponent = UiState.CardGameObjects[actionResult.Card];
                cardComponent.OnClick = () => Play(cardComponent);
            }

            return actionResult;
        }

        private void BringToTop(CardComponent cardComponent)
        {
            UiState.GameObjects.Remove(cardComponent);
            UiState.GameObjects.Add(cardComponent);
        }
    }
}

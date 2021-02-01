using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGame.UI
{
    public class BlazorGameController
    {
        private readonly InGameUiBuilder inGameUiBuilder;
        private readonly Game game;
        private readonly ActionResultMessageMapper actionResultMessageMapper;
        public UiState UiState { get; set; }
        public Turn CurrentTurn => game.GetCurrentTurn();
        
        public BlazorGameController(InGameUiBuilder inGameUiBuilder, Game game, ActionResultMessageMapper actionResultMessageMapper)
        {
            this.inGameUiBuilder = inGameUiBuilder;
            this.game = game;
            this.actionResultMessageMapper = actionResultMessageMapper;
        }

        public async void Deal()
        {
            game.Deal();
            UiState = await inGameUiBuilder.Build(this);
            UiState.CurrentGamePhase = game.GameState.CurrentGamePhase;
        }

        public bool Play(CardComponent cardComponent)
        {
            if (!cardComponent.IsTurnedUp)
                return false;

            var actionResult = game.Play(CurrentTurn.PlayerToPlay, cardComponent.Card);

            if (actionResult.IsSuccess)
            {
                BringToTop(cardComponent);
                UiState.HasError = false;
                UiState.ErrorMessage = null;
            }
            else
            {
                var errorMessage = actionResultMessageMapper.ToString(actionResult.MessageKey);
                if (actionResult.MessageKey == ActionResultMessageKey.InvalidPlay)
                    errorMessage = errorMessage.Replace("{Card}", cardComponent.Card.ToString());

                UiState.HasError = true;
                UiState.ErrorMessage = errorMessage;
            }

            return actionResult.IsSuccess;
        }

        public void SelectSuit(Suit suit)
        {
            var actionResult = game.SelectSuit(CurrentTurn.PlayerToPlay, suit);

            if (actionResult.IsSuccess)
            {
                UiState.HasError = false;
                UiState.ErrorMessage = null;
            }
            else
            {
                UiState.HasError = true;
                UiState.ErrorMessage = actionResultMessageMapper.ToString(actionResult.MessageKey);
            }

        }

        public ActionResultWithCard Take()
        {
            var actionResult = game.Take(CurrentTurn.PlayerToPlay);

            if (actionResult.IsSuccess)
            {
                UiState.HasError = false;
                UiState.ErrorMessage = null;
            }
            else
            {
                UiState.HasError = true;
                UiState.ErrorMessage = actionResultMessageMapper.ToString(actionResult.MessageKey);
            }

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

using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGame.UI
{
    public class BlazorGameController
    {
        private readonly InGameUiBuilder inGameUiBuilder;
        private readonly Game game;
        private readonly ActionResultMessageMapper actionResultMessageMapper;
        public CurrentTurn CurrentTurn => game.GameState.CurrentTurn;
        public PreviousTurnResult PreviousTurnResult => game.GameState.PreviousTurnResult;
        public CardCollection AllCards => game.GameState.CurrentTable.AllCards;
        
        public UiState UiState { get; set; }
        public readonly Dictionary<LabelNames, LabelComponent> Labels = new Dictionary<LabelNames, LabelComponent>();
        public readonly Dictionary<ButtonNames, ButtonComponent> Buttons = new Dictionary<ButtonNames, ButtonComponent>();

        public BlazorGameController(InGameUiBuilder inGameUiBuilder, Game game, ActionResultMessageMapper actionResultMessageMapper)
        {
            this.inGameUiBuilder = inGameUiBuilder;
            this.game = game;
            this.actionResultMessageMapper = actionResultMessageMapper;
        }

        public async void Deal()
        {
            game.Deal();
            UiState = await inGameUiBuilder.Build(this, game.GameState);

            Labels.Add(LabelNames.Turn, UiState.GameObjects.First(x => x.Tag == LabelNames.Turn.ToString()) as LabelComponent);
            Labels.Add(LabelNames.PlayerToPlay, UiState.GameObjects.First(x => x.Tag == LabelNames.PlayerToPlay.ToString()) as LabelComponent);
            Labels.Add(LabelNames.InvalidPlay, UiState.GameObjects.First(x => x.Tag == LabelNames.InvalidPlay.ToString()) as LabelComponent);
            Labels.Add(LabelNames.SelectedSuit, UiState.GameObjects.First(x => x.Tag == LabelNames.SelectedSuit.ToString()) as LabelComponent);
            
            Buttons.Add(ButtonNames.Take, UiState.GameObjects.First(x => x.Tag == ButtonNames.Take.ToString()) as ButtonComponent);

            Buttons.Add(ButtonNames.Clubs, UiState.GameObjects.First(x => x.Tag == ButtonNames.Clubs.ToString()) as ButtonComponent);
            Buttons.Add(ButtonNames.Diamonds, UiState.GameObjects.First(x => x.Tag == ButtonNames.Diamonds.ToString()) as ButtonComponent);
            Buttons.Add(ButtonNames.Hearts, UiState.GameObjects.First(x => x.Tag == ButtonNames.Hearts.ToString()) as ButtonComponent);
            Buttons.Add(ButtonNames.Spades, UiState.GameObjects.First(x => x.Tag == ButtonNames.Spades.ToString()) as ButtonComponent);
        }

        public bool Play(CardComponent cardComponent)
        {
            if (!cardComponent.IsTurnedUp)
                return false;

            var actionResult = game.Play(new PlayContext(CurrentTurn.PlayerToPlay, CardsUtils.Cards(cardComponent.Card)));

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
            var actionResult = game.SelectSuit(new SelectSuitContext(CurrentTurn.PlayerToPlay, suit));

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

        public ActionResult Take()
        {
            var actionResult = game.Take(new TakeContext(CurrentTurn.PlayerToPlay));

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
                var cardComponent = UiState.CardGameObjects[game.GameState.PreviousTurnResult.TakenCard];
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

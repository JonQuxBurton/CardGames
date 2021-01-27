using System;
using System.Collections.Generic;
using SheddingCardGames.Domain;
using Action = SheddingCardGames.Domain.Action;

namespace SheddingCardGame.UI
{
    public class GameController
    {
        public GameState GameState { get; private set; }
        public UiState UiState { get; set; }
        public Turn CurrentTurn => game.GetCurrentTurn();

        private readonly Game game;
        private readonly InGameUiBuilder inGameUiBuilder;
        private readonly Dictionary<ActionResultMessageKey, string> errorMessages;

        public GameController(Game game, InGameUiBuilder inGameUiBuilder)
        {
            this.game = game;
            this.inGameUiBuilder = inGameUiBuilder;

            GameState = new GameState {CurrentGamePhase = GamePhase.New};

            errorMessages = new Dictionary<ActionResultMessageKey, string>
            {
                {ActionResultMessageKey.CardIsNotInPlayersHand, "Card is not in the current Players hand"},
                {ActionResultMessageKey.InvalidPlay, "You cannot play the Card: {Card}"},
                {ActionResultMessageKey.NotPlayersTurn, "It is not this Player's turn"}
            };
        }

        public async void Deal()
        {
            UiState = await inGameUiBuilder.Build(this);
            GameState = new GameState {CurrentGamePhase = GamePhase.InGame, HasError = false, ErrorMessage = null};
        }

        public void SelectSuit(Suit suit)
        {
            var playerNumber = game.GetCurrentTurn().PlayerToPlay;
            var actionResult = game.SelectSuit(playerNumber, suit);

            if (actionResult.IsSuccess)
            {
                GameState.HasError = false;
                GameState.ErrorMessage = "";
            }
            else
            {
                GameState.HasError = true;
                GameState.ErrorMessage = errorMessages[actionResult.MessageKey];
            }
        }

        public void Play(CardComponent cardComponent)
        {
            if (!cardComponent.IsTurnedUp)
                return;

            var actionResult = game.Play(game.GetCurrentTurn().PlayerToPlay, cardComponent.Card);

            if (actionResult.IsSuccess)
            {
                GameState.HasError = false;
                GameState.ErrorMessage = null;
                BringToTop(cardComponent);
            }
            else
            {
                GameState.HasError = true;
                
                var errorMessage = errorMessages[actionResult.MessageKey];
                if (actionResult.MessageKey == ActionResultMessageKey.InvalidPlay)
                    errorMessage = errorMessage.Replace("{Card}", cardComponent.Card.ToString(), StringComparison.InvariantCultureIgnoreCase);

                GameState.ErrorMessage = errorMessage;
            }
        }

        public void Take()
        {
            if (game.GetCurrentTurn().NextAction != Action.Take)
            {
                GameState.HasError = true;
                GameState.ErrorMessage = "You cannot Take a Card at this time";
                return;
            }

            var actionResult = game.Take(game.GetCurrentTurn().PlayerToPlay);

            if (actionResult.IsSuccess)
            {
                var cardComponent = UiState.CardGameObjects[actionResult.Card];
                cardComponent.OnClick = () => Play(cardComponent);
            }
            else
            {
                GameState.HasError = true;
                GameState.ErrorMessage = errorMessages[actionResult.MessageKey];
            }
        }
        
        private void BringToTop(CardComponent cardComponent)
        {
            UiState.GameObjects.Remove(cardComponent);
            UiState.GameObjects.Add(cardComponent);
        }
    }
}
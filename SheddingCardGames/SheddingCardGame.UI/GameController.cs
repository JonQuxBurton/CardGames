using System;
using System.Collections.Generic;
using SheddingCardGames.Domain;

namespace SheddingCardGame.UI
{
    public interface IGameController
    {
        void Deal();
        void SelectSuit(Suit suit);
        bool Play(CardComponent cardComponent);
        ActionResultWithCard Take();
    }

    public class GameController : IGameController
    {
        public GameState GameState { get; private set; }
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
                {ActionResultMessageKey.NotPlayersTurn, "It is not this Player's turn"},
                {ActionResultMessageKey.InvalidTake, "You cannot Take a Card at this time"}
            };
        }

        public void Deal()
        {
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

        public bool Play(CardComponent cardComponent)
        {
            if (!cardComponent.IsTurnedUp)
                return false;

            var actionResult = game.Play(game.GetCurrentTurn().PlayerToPlay, cardComponent.Card);

            if (actionResult.IsSuccess)
            {
                GameState.HasError = false;
                GameState.ErrorMessage = null;
                return true;
            }

            GameState.HasError = true;
                
            var errorMessage = errorMessages[actionResult.MessageKey];
            if (actionResult.MessageKey == ActionResultMessageKey.InvalidPlay)
                errorMessage = errorMessage.Replace("{Card}", cardComponent.Card.ToString(), StringComparison.InvariantCultureIgnoreCase);

            GameState.ErrorMessage = errorMessage;
            return false;
        }

        public ActionResultWithCard Take()
        {
            var actionResult = game.Take(game.GetCurrentTurn().PlayerToPlay);

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

            return actionResult;
        }
    }
}
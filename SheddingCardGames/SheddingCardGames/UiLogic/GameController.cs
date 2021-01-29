using System.Collections.Generic;
using SheddingCardGame.UI;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class GameController : IGameController
    {
        private readonly Dictionary<ActionResultMessageKey, string> errorMessages;

        private readonly Game game;

        public GameController(Game game)
        {
            this.game = game;

            GameState = new GameState {CurrentGamePhase = GamePhase.New};

            errorMessages = new Dictionary<ActionResultMessageKey, string>
            {
                {ActionResultMessageKey.CardIsNotInPlayersHand, "Card is not in the current Players hand"},
                {ActionResultMessageKey.InvalidPlay, "You cannot play the Card: {Card}"},
                {ActionResultMessageKey.NotPlayersTurn, "It is not this Player's turn"},
                {ActionResultMessageKey.InvalidTake, "You cannot Take a Card at this time"}
            };
        }

        public GameState GameState { get; private set; }
        public Turn CurrentTurn => game.GetCurrentTurn();

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

        public bool Play(Card card)
        {
            var actionResult = game.Play(game.GetCurrentTurn().PlayerToPlay, card);

            if (actionResult.IsSuccess)
            {
                GameState.HasError = false;
                GameState.ErrorMessage = null;
                return true;
            }

            GameState.HasError = true;

            var errorMessage = errorMessages[actionResult.MessageKey];
            if (actionResult.MessageKey == ActionResultMessageKey.InvalidPlay)
                errorMessage = errorMessage.Replace("{Card}", card.ToString());

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
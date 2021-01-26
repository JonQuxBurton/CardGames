using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SheddingCardGames;
using Action = SheddingCardGames.Action;

namespace SheddingCardGame.UI.Pages
{
    public class GameController
    {
        private readonly Game game;
        private readonly InGameUiBuilder inGameUiBuilder;

        public UiState UiState { get; set; }
        public GamePhase CurrentGamePhase = GamePhase.New;

        private readonly Dictionary<ActionResultMessageKey, string> errorMessages = new Dictionary<ActionResultMessageKey, string>();
            
        public GameController(Game game, InGameUiBuilder inGameUiBuilder)
        {
            this.game = game;
            this.inGameUiBuilder = inGameUiBuilder;

            errorMessages = new Dictionary<ActionResultMessageKey, string>
            {
                {ActionResultMessageKey.CardIsNotInPlayersHand, "Card is not in the current Players hand"},
                {ActionResultMessageKey.InvalidPlay, "You cannot play the Card: {Card}"},
                {ActionResultMessageKey.NotPlayersTurn, "It is not this Player's turn"}
            };
        }

        public Turn CurrentTurn => game.GetCurrentTurn();

        public async ValueTask Invoke(string actionName)
        {
            Console.WriteLine($"Invoke {actionName}");

            if (actionName == "Deal")
            {
                UiState = await inGameUiBuilder.Build(this);
                CurrentGamePhase = GamePhase.InGame;
                UiState.HasError = false;
                UiState.ErrorMessage = null;
            }
            else if (actionName == "Take")
            {
                Take();
            }
            else
            {
                var suit = (Suit)Enum.Parse(typeof(Suit), actionName);
                SelectSuit(suit);
            }
        }

        private void SelectSuit(Suit suit)
        {
            var playerNumber = game.GetCurrentTurn().PlayerToPlay;
            var actionResult = game.SelectSuit(playerNumber, suit);

            if (actionResult.IsSuccess)
            {
                UiState.HasError = false;
                UiState.ErrorMessage = "";
            }
            else
            {
                UiState.HasError = true;
                UiState.ErrorMessage = errorMessages[actionResult.MessageKey];
            }
        }
        
        private void Take()
        {
            if (game.GetCurrentTurn().NextAction != Action.Take)
            {
                UiState.HasError = true;
                UiState.ErrorMessage = "You cannot Take a Card at this time";
                return;
            }

            var actionResult = game.Take(game.GetCurrentTurn().PlayerToPlay);

            if (actionResult.IsSuccess)
            {
                UiState.CardGameObjects[actionResult.Card].Tag = $"{actionResult.Card}";
            }
            else
            {
                UiState.HasError = true;
                UiState.ErrorMessage = errorMessages[actionResult.MessageKey];
            }
        }

        private void Play(CardComponent cardComponent)
        {
            // Bring to top
            UiState.GameObjects.Remove(cardComponent);
            UiState.GameObjects.Add(cardComponent);
        }

        public void CardClick(CardComponent cardComponent)
        {
            if (cardComponent.Tag == "StockPile")
            {
                Console.WriteLine("Clicked on StockPile");
                Take();

                return;
            }
            
            if (!cardComponent.IsTurnedUp)
                return;
            
            Console.WriteLine($"Clicked on {cardComponent.Card}");
            var actionResult = game.Play(game.GetCurrentTurn().PlayerToPlay, cardComponent.Card);
            Console.WriteLine($"Is valid?:  {actionResult}");

            if (actionResult.IsSuccess)
            {
                UiState.HasError = false;
                UiState.ErrorMessage = null;
                Play(cardComponent);
            }
            else
            {
                UiState.HasError = true;
                
                var errorMessage = errorMessages[actionResult.MessageKey];
                if (actionResult.MessageKey == ActionResultMessageKey.InvalidPlay)
                    errorMessage = errorMessage.Replace("{Card}", cardComponent.Card.ToString(), StringComparison.InvariantCultureIgnoreCase);
                
                UiState.ErrorMessage = errorMessage;
            }
        }
    }
}
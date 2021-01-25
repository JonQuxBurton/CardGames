using System;
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

        public GameController(Game game, InGameUiBuilder inGameUiBuilder)
        {
            this.game = game;
            this.inGameUiBuilder = inGameUiBuilder;
        }

        public Turn CurrentTurn => game.GetCurrentTurn();

        public async ValueTask Invoke(string actionName)
        {
            Console.WriteLine($"Invoke {actionName}");

            if (actionName == "Deal")
            {
                UiState = await inGameUiBuilder.Build(this);
                CurrentGamePhase = GamePhase.InGame;
            }
            else if (actionName == "Take")
            {
                Take();
            }
            else if (actionName == "Clubs")
            {
                game.SelectSuit(Suit.Clubs);
            }
            else if (actionName == "Diamonds")
            {
                game.SelectSuit(Suit.Diamonds);
            }
            else if (actionName == "Hearts")
            {
                game.SelectSuit(Suit.Hearts);
            }
            else if (actionName == "Spades")
            {
                game.SelectSuit(Suit.Spades);
            }
            UiState.InvalidPlayCard = null;
        }

        private void Take()
        {
            var takenCard = game.Take();
            UiState.CardGameObjects[takenCard].Tag = $"{takenCard}";
            UiState.IsInvalidTake = false;
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
                if (game.GetCurrentTurn().NextAction == Action.Take)
                    Take();
                else
                    UiState.IsInvalidTake = true;

                return;
            }
            
            if (!cardComponent.IsTurnedUp)
                return;
            
            Console.WriteLine($"Clicked on {cardComponent.Card}");
            var isValid = game.Play(cardComponent.Card);
            Console.WriteLine($"Is valid?:  {isValid}");

            if (isValid)
            {
                UiState.InvalidPlayCard = null;
                UiState.IsInvalidTake = false;
                Play(cardComponent);
            }
            else
            {
                UiState.InvalidPlayCard = cardComponent;
            }
        }
    }
}
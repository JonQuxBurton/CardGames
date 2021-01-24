using System;
using System.Threading.Tasks;
using SheddingCardGames;
using Action = SheddingCardGames.Action;

namespace SheddingCardGame.UI.Pages
{
    public class GameController
    {
        private readonly CrazyEights crazyEights;
        private readonly Game game;

        public GamePhase CurrentGamePhase = GamePhase.New;
        public CardComponent InvalidPlayCard;

        public GameController(CrazyEights crazyEights, Game game)
        {
            this.crazyEights = crazyEights;
            this.game = game;
        }

        public Turn CurrentTurn => game.GetCurrentTurn();

        public async ValueTask Invoke(string actionName)
        {
            Console.WriteLine($"Invoke {actionName}");

            if (actionName == "Deal")
            {
                await crazyEights.Deal();
                CurrentGamePhase = GamePhase.InGame;
            }
            else if (actionName == "Take")
            {
                var taken = game.Take();
                crazyEights.Take(taken);
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
            InvalidPlayCard = null;
        }

        public void CardClick(CardComponent cardComponent)
        {
            if (cardComponent.Tag == "StockPile")
            {
                Console.WriteLine("Clicked on StockPile");
                if (game.GetCurrentTurn().NextAction == Action.Take)
                {
                    var taken = game.Take();
                    crazyEights.Take(taken);
                }
                else
                {
                    crazyEights.InvalidTake();
                }

                return;
            }
            
            if (!cardComponent.IsTurnedUp)
                return;
            
            Console.WriteLine($"Clicked on {cardComponent.Card}");
            var isValid = game.Play(cardComponent.Card);
            Console.WriteLine($"Is valid?:  {isValid}");

            if (isValid)
            {
                InvalidPlayCard = null;
                crazyEights.Play(cardComponent);
            }
            else
            {
                InvalidPlayCard = cardComponent;
            }
        }
    }
}
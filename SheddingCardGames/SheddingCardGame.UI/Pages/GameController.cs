using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using SheddingCardGames;

namespace SheddingCardGame.UI.Pages
{
    public class GameController
    {
        private readonly Game game;
        private readonly List<IGameObject> gameObjects;
        private readonly List<IGameObject> inGameGameObjects;
        private readonly ElementReference cardsSpriteSheet;
        CardComponent discardCardComponent;

        public GameController(Game game, List<IGameObject> gameObjects, List<IGameObject> inGameGameObjects, ElementReference cardsSpriteSheet)
        {
            this.game = game;
            this.gameObjects = gameObjects;
            this.inGameGameObjects = inGameGameObjects;
            this.cardsSpriteSheet = cardsSpriteSheet;
        }
        
        public void CardClick(Card card)
        {
            var isValid = game.Play(card);
            
            Refresh();
        }

        public Turn CurrentTurn => game.GetCurrentTurn();

        public void Refresh()
        {
            var cardWidth = 154;
            var cardHeight = 240;

            var player2LabelY = 10;
            var player2HandY = player2LabelY + 30;
            var discardPileY = player2HandY + cardHeight;
            var player1HandY = discardPileY + cardHeight;
            var player1LabelY = player1HandY + cardHeight;

            if (this.discardCardComponent != null)
            {
                gameObjects.Remove(discardCardComponent);
                inGameGameObjects.Remove(discardCardComponent);
            }

            //inGameGameObjects.Clear();

            discardCardComponent = new CardComponent(this, CurrentTurn.DiscardPile.CardToMatch, new Sprite(cardsSpriteSheet, new Size(cardWidth, 240), new Point(cardWidth, discardPileY)), true);
            inGameGameObjects.Add(discardCardComponent);
            gameObjects.Add(discardCardComponent);

        }
    }


}
using System.Drawing;
using Microsoft.AspNetCore.Components;
using SheddingCardGames.Domain;

namespace SheddingCardGame.UI
{
    public class CardComponentBuilder
    {
        private readonly ElementReference cardsSpriteSheet;
        private readonly BlazorGameController gameController;
        private readonly UiState uiState;

        public CardComponentBuilder(BlazorGameController gameController, UiState uiState,
            ElementReference cardsSpriteSheet)
        {
            this.gameController = gameController;
            this.uiState = uiState;
            this.cardsSpriteSheet = cardsSpriteSheet;
        }

        public void Build(Cursor cursor, Card card)
        {
            var cardWidth = 154;
            var cardHeight = 240;

            var component = new CardComponent(card,
                new Sprite(cardsSpriteSheet, new Size(cardWidth, cardHeight), new Point(cursor.X, cursor.Y)), false);
            component.OnClick = () => gameController.Play(component);
            uiState.CardGameObjects.Add(card, component);
            uiState.GameObjects.Add(component);
            cursor.NextRow();
        }
    }
}
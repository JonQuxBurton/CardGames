using System.Drawing;
using Microsoft.AspNetCore.Components;
using SheddingCardGames.Domain;

namespace SheddingCardGame.UI
{
    public class CardComponentBuilder
    {
        private readonly ElementReference cardsSpriteSheet;
        private readonly Config config;
        private readonly BlazorGameController gameController;
        private readonly UiState uiState;

        public CardComponentBuilder(Config config, BlazorGameController gameController, UiState uiState,
            ElementReference cardsSpriteSheet)
        {
            this.config = config;
            this.gameController = gameController;
            this.uiState = uiState;
            this.cardsSpriteSheet = cardsSpriteSheet;
        }

        public void Build(Cursor cursor, Card card)
        {
            var component = new CardComponent(config, card,
                new Sprite(cardsSpriteSheet, new Size(config.CardWidth, config.CardHeight), new Point(cursor.X, cursor.Y)), false);
            component.OnClick = () => gameController.Play(component);
            uiState.CardGameObjects.Add(card, component);
            uiState.GameObjects.Add(component);
            cursor.NextRow();
        }
    }
}
using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGame.UI
{
    public partial class InGameUiBuilder
    {
        private readonly ElementReference cardsSpriteSheet;
        private readonly Canvas2DContext context;
        private UiState uiState;
        private ButtonBuilder buttonBuilder;
        private LabelBuilder labelBuilder;
        private CardComponentBuilder cardComponentBuilder;

        public InGameUiBuilder(Canvas2DContext context,
            ElementReference cardsSpriteSheet)
        {
            this.context = context;
            this.cardsSpriteSheet = cardsSpriteSheet;
            
        }

        public async Task<UiState> Build(BlazorGameController gameController, GameState gameState)
        {
            uiState = new UiState(gameState){ CurrentGamePhase = GamePhase.InGame};
            buttonBuilder = new ButtonBuilder(uiState);
            labelBuilder = new LabelBuilder(uiState);
            cardComponentBuilder = new CardComponentBuilder(gameController, uiState, cardsSpriteSheet);

            var cardHeight = 240;

            var player2LabelY = 10;
            var player2HandY = player2LabelY + 30;
            var discardPileY = player2HandY + cardHeight;
            var player1HandY = discardPileY + cardHeight;
            var player1LabelY = player1HandY + cardHeight;


            var player2LabelX = await GetXForText(context, "24px verdana", "Player 2", 1200);
            uiState.GameObjects.Add(new LabelComponent("Player 2", new Point(player2LabelX, player2LabelY), true));
            var player1LabelX = await GetXForText(context, "24px verdana", "Player 1", 1200);
            uiState.GameObjects.Add(new LabelComponent("Player 1", new Point(player1LabelX, player1LabelY), true));

            var statusAreaPosition = new Point(360, discardPileY + 60);
            SetupStatusArea(gameController, statusAreaPosition);
            
            var cursor = new Cursor(0, 0, 0);
            
            foreach (var card in gameController.AllCards.Cards) 
                cardComponentBuilder.Build(cursor, card);

            return uiState;
        }

        public void SetupStatusArea(BlazorGameController gameController, Point statusAreaPosition)
        {
            var rowHeight = 30;

            var cursor = new Cursor(statusAreaPosition.X, statusAreaPosition.Y, rowHeight);
            
            labelBuilder.Build(cursor, LabelNames.Turn);
            labelBuilder.Build(cursor, LabelNames.PlayerToPlay);
            labelBuilder.Build(cursor, LabelNames.SelectedSuit);
            labelBuilder.Build(cursor, LabelNames.InvalidPlay);
            buttonBuilder.Build(cursor, ButtonNames.Take, "TAKE", () => gameController.Take());

            rowHeight = 40;
            cursor = new Cursor(statusAreaPosition.X + 500, statusAreaPosition.Y, rowHeight);
            buttonBuilder.Build(cursor, ButtonNames.Clubs, ButtonNames.Clubs.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Clubs));
            buttonBuilder.Build(cursor, ButtonNames.Diamonds, ButtonNames.Diamonds.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Diamonds));
            buttonBuilder.Build(cursor, ButtonNames.Hearts, ButtonNames.Hearts.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Hearts));
            buttonBuilder.Build(cursor, ButtonNames.Spades, ButtonNames.Spades.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Spades));
        }

        public async Task<int> GetXForText(Canvas2DContext context, string fontInfo, string text, int areaWidth)
        {
            await context.SetFontAsync(fontInfo);
            var metrics = await context.MeasureTextAsync(text);
            return (int) Math.Round((areaWidth - metrics.Width) / 2);
        }
    }
}
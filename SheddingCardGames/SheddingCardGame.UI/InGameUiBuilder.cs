using System;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGame.UI
{
    public class InGameUiBuilder
    {
        private readonly ElementReference cardsSpriteSheet;
        private readonly Config config;
        private readonly Canvas2DContext context;
        private UiState uiState;
        private ButtonComponentBuilder buttonComponentBuilder;
        private LabelComponentBuilder labelComponentBuilder;
        private CardComponentBuilder cardComponentBuilder;

        public InGameUiBuilder(Config config, Canvas2DContext context,
            ElementReference cardsSpriteSheet)
        {
            this.config = config;
            this.context = context;
            this.cardsSpriteSheet = cardsSpriteSheet;
        }

        public async Task<UiState> Build(BlazorGameController gameController, GameState gameState)
        {
            uiState = new UiState(gameState){ CurrentGamePhase = GamePhase.InGame};
            buttonComponentBuilder = new ButtonComponentBuilder(config, uiState);
            labelComponentBuilder = new LabelComponentBuilder(config, uiState);
            cardComponentBuilder = new CardComponentBuilder(config, gameController, uiState, cardsSpriteSheet);

            var player2LabelX = await GetXForCentredText(context, config.Font, gameState.CurrentTable.Players[1].Name, config.TableSection.Width);
            var player1LabelX = await GetXForCentredText(context, config.Font, gameState.CurrentTable.Players[0].Name, config.TableSection.Width);
            
            labelComponentBuilder.Build(new Cursor(player2LabelX, config.TopPlayerInfoSection.Y), LabelNames.Player2, gameState.CurrentTable.Players[1].Name);
            labelComponentBuilder.Build(new Cursor(player1LabelX, config.BottomPlayerInfoSection.Y), LabelNames.Player1, gameState.CurrentTable.Players[0].Name);

            SetupInfoSection(gameController);
            
            foreach (var card in gameController.AllCards.Cards) 
                cardComponentBuilder.Build(new Cursor(0, 0), card);

            return uiState;
        }

        public void SetupInfoSection(BlazorGameController gameController)
        {
            var cursor = new Cursor(config.InfoSection.X, config.InfoSection.Y, config.LabelHeight);
            labelComponentBuilder.Build(cursor, LabelNames.Turn);
            labelComponentBuilder.Build(cursor, LabelNames.PlayerToPlay);
            labelComponentBuilder.Build(cursor, LabelNames.SelectedSuit);
            labelComponentBuilder.Build(cursor, LabelNames.InvalidPlay);
            buttonComponentBuilder.Build(cursor, ButtonNames.Take, ButtonNames.Take.ToString().ToUpper(), () => gameController.Take());

            cursor = new Cursor(config.ActionsSection.X, config.ActionsSection.Y, config.LabelHeight);
            buttonComponentBuilder.Build(cursor, ButtonNames.Clubs, ButtonNames.Clubs.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Clubs));
            buttonComponentBuilder.Build(cursor, ButtonNames.Diamonds, ButtonNames.Diamonds.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Diamonds));
            buttonComponentBuilder.Build(cursor, ButtonNames.Hearts, ButtonNames.Hearts.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Hearts));
            buttonComponentBuilder.Build(cursor, ButtonNames.Spades, ButtonNames.Spades.ToString().ToUpper(), () => gameController.SelectSuit(Suit.Spades));
        }

        private async Task<int> GetXForCentredText(Canvas2DContext context, string fontInfo, string text, int areaWidth)
        {
            await context.SetFontAsync(fontInfo);
            var metrics = await context.MeasureTextAsync(text);
            return (int) Math.Round((areaWidth - metrics.Width) / 2);
        }
    }
}
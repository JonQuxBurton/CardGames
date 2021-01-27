using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Microsoft.AspNetCore.Components;
using SheddingCardGames.Domain;

namespace SheddingCardGame.UI
{
    public class InGameUiBuilder
    {
        private readonly ElementReference cardsSpriteSheet;
        private readonly CardCollection deck;
        private readonly Canvas2DContext context;
        private UiState uiState;

        public InGameUiBuilder(Canvas2DContext context,
            ElementReference cardsSpriteSheet, CardCollection deck)
        {
            this.context = context;
            this.cardsSpriteSheet = cardsSpriteSheet;
            this.deck = deck;
        }

        public async Task<UiState> Build(GameController gameController)
        {
            uiState = new UiState();

            var cardWidth = 154;
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

            var deckX = 0;
            var deckY = 0;
            var counter = 0;
            foreach (var card in deck.Cards)
            {
                var cardComponent = new CardComponent(gameController, card,
                    new Sprite(cardsSpriteSheet, new Size(cardWidth, 240), new Point(deckX, deckY)), false);
                cardComponent.OnClick = () => gameController.Play(cardComponent);
                uiState.CardGameObjects.Add(card, cardComponent);
                deckX += 30;
                counter++;
                if (counter == 13)
                {
                    counter = 0;
                    deckY += cardHeight;
                    deckX = 0;
                }
            }

            uiState.GameObjects.AddRange(uiState.CardGameObjects.Values);

            return uiState;
        }

        public void SetupStatusArea(GameController gameController, Point statusAreaPosition)
        {
            var y = statusAreaPosition.Y;
            var rowHeight = 30;

            uiState.TurnLabel = new LabelComponent($"Turn {gameController.CurrentTurn.TurnNumber}",
                new Point(statusAreaPosition.X, y), true);
            uiState.GameObjects.Add(uiState.TurnLabel);
            y += rowHeight;

            uiState.PlayerToPlayLabel =
                new LabelComponent(
                    $"Player {gameController.CurrentTurn.PlayerToPlay} to {gameController.CurrentTurn.NextAction}",
                    new Point(statusAreaPosition.X, y), true);
            uiState.GameObjects.Add(uiState.PlayerToPlayLabel);
            y += rowHeight;
            
            uiState.SelectedSuitLabel =
                new LabelComponent("Selected Suit: X",
                    new Point(statusAreaPosition.X, y), true);
            uiState.GameObjects.Add(uiState.SelectedSuitLabel);
            y += rowHeight;

            uiState.InvalidPlayLabel =
                new LabelComponent("You cannot play the Card: 1 Clubs", new Point(statusAreaPosition.X, y), false);
            uiState.GameObjects.Add(uiState.InvalidPlayLabel);
            y += rowHeight;
            
            uiState.TakeButton = new ButtonComponent("Take", new Point(statusAreaPosition.X, y), true, () => gameController.Take());
            uiState.GameObjects.Add(uiState.TakeButton);
            y += rowHeight;

            var x = statusAreaPosition.X + 500;
            y = statusAreaPosition.Y;
            uiState.ClubsButton = new ButtonComponent("Clubs", new Point(x, y), true, () => gameController.SelectSuit(Suit.Clubs));
            uiState.GameObjects.Add(uiState.ClubsButton);
            y += rowHeight;
            
            uiState.DiamondsButton = new ButtonComponent("Diamonds", new Point(x, y), true, () => gameController.SelectSuit(Suit.Diamonds));
            uiState.GameObjects.Add(uiState.DiamondsButton);
            y += rowHeight;
            
            uiState.HeartsButton = new ButtonComponent("Hearts", new Point(x, y), true, () => gameController.SelectSuit(Suit.Hearts));
            uiState.GameObjects.Add(uiState.HeartsButton);
            y += rowHeight;
            
            uiState.SpadesButton = new ButtonComponent("Spades", new Point(x, y), true, () => gameController.SelectSuit(Suit.Spades));
            uiState.GameObjects.Add(uiState.SpadesButton);
            y += rowHeight;
        }

        public async Task<int> GetXForText(Canvas2DContext context, string fontInfo, string text, int areaWidth)
        {
            await context.SetFontAsync(fontInfo);
            var metrics = await context.MeasureTextAsync(text);
            return (int) Math.Round((areaWidth - metrics.Width) / 2);
        }
    }
}
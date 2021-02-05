using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.Model;

namespace SheddingCardGame.UI
{
    public class ButtonComponent : GameObject, IGameObject
    {
        private readonly Config config;
        public Action ButtonAction { get; set; }
        public string Label { get; }
        public Point Position { get; }

        private Rectangle? boundingBox;
        private bool isActive;

        public ButtonComponent(Config config, string label, Point position, bool isVisible, Action buttonAction)
        {
            this.config = config;
            ButtonAction = buttonAction;
            Label = label;
            Tag = label;
            Position = position;
            IsVisible = isVisible;
        }

        public string Tag { get; set; }

        public ValueTask Update(InputState inputState)
        {
            if (boundingBox.HasValue && boundingBox.Value.Contains(inputState.MouseCoords))
                isActive = true;
            else
                isActive = false;

            if (inputState.IsMouseClicked)
                ButtonAction();
            
            return new ValueTask();
        }

        public async ValueTask Render(Canvas2DContext context)
        {
            var padding = config.ButtonPadding;
            await context.SetFontAsync(config.Font);

            if (boundingBox == null)
            {
                TextMetrics metrics = await context.MeasureTextAsync(Label);
                var width = metrics.Width + padding*2;
                var height = await GetTextHeightAsync(context, config.Font) + padding * 2;
                boundingBox = new Rectangle(Position.X, Position.Y, Round(width), Round(height));
            }

            await context.SetLineWidthAsync(config.ButtonBorderWidth);
            await context.SetStrokeStyleAsync(config.FontColour);
            await context.StrokeRectAsync(boundingBox.Value.X, boundingBox.Value.Y, boundingBox.Value.Width, boundingBox.Value.Height);
            await context.SetFillStyleAsync(config.FontColour);
            await context.FillTextAsync($"{Label}", Position.X + padding, Position.Y + padding);

            if (isActive)
            {
                var box = boundingBox.Value;
                await context.BeginPathAsync();
                await context.SetStrokeStyleAsync(config.HighlightColour);
                await context.SetLineWidthAsync(config.HighlightWidth);
                await context.StrokeRectAsync(box.X, box.Y, box.Width, box.Height);
            }
        }

        private int Round(double d)
        {
            return (int)Math.Round(d);
        }

        private async Task<double> GetTextHeightAsync(Canvas2DContext context, string fontInfo)
        {
            await context.SetFontAsync(fontInfo);
            TextMetrics metrics = await context.MeasureTextAsync("M");
            return metrics.Width;
        }

        public bool IsHit(Point mousePosition)
        {
            if (boundingBox == null)
                return false;

            return boundingBox.Value.Contains(mousePosition);
        }

        public void Show()
        {
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }
    }
}
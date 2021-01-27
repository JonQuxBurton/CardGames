using System;
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using Blazor.Extensions.Canvas.Model;

namespace SheddingCardGame.UI
{
    public class ButtonComponent : GameObject, IGameObject
    {
        public Action ButtonAction { get; set; }
        public string Label { get; }
        public Point Position { get; }
        public Rectangle? BoundingBox => boundingBox;

        private Rectangle? boundingBox;
        private bool isActive;

        public ButtonComponent(string label, Point position, bool isVisible, Action buttonAction)
        {
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
            var fontInfo = "24px verdana";
            var padding = 5;
            await context.SetFontAsync(fontInfo);

            if (boundingBox == null)
            {
                TextMetrics metrics = await context.MeasureTextAsync(Label);
                var width = metrics.Width + padding*2;
                var height = await GetTextHeightAsync(context, fontInfo) + padding * 2;
                boundingBox = new Rectangle(Position.X, Position.Y, Round(width), Round(height));
            }

            await context.SetLineWidthAsync(2);
            await context.SetStrokeStyleAsync("White");
            await context.StrokeRectAsync(boundingBox.Value.X, boundingBox.Value.Y, boundingBox.Value.Width, boundingBox.Value.Height);
            await context.SetFillStyleAsync("White");
            await context.FillTextAsync($"{Label}", Position.X + padding, Position.Y + padding);

            if (isActive)
            {
                var box = boundingBox.Value;
                await context.BeginPathAsync();
                await context.SetStrokeStyleAsync($"rgb(255,0,0)");
                await context.SetLineWidthAsync(4);
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
    }
}
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace SheddingCardGame.UI
{
    public class LabelComponent : GameObject, IGameObject
    {
        private readonly Config config;
        public string Label { get; set; }
        public Point Position { get; }

        public LabelComponent(Config config, string label, Point position, bool isVisible)
        {
            this.config = config;
            Label = label;
            Tag = label;
            Position = position;
            IsVisible = isVisible;
        }

        public string Tag { get; set; }

        public ValueTask Update(InputState inputState)
        {
            return new ValueTask();
        }

        public async ValueTask Render(Canvas2DContext context)
        {
            await context.SetFontAsync(config.Font);
            await context.SetStrokeStyleAsync(config.FontColour);
            await context.SetFillStyleAsync(config.FontColour);
            await context.FillTextAsync($"{Label}", Position.X, Position.Y);
        }

        public bool IsHit(Point mousePosition)
        {
            return false;
        }

        public void Show(string message)
        {
            Label = message;
            IsVisible = true;
        }

        public void Hide()
        {
            IsVisible = false;
        }
    }
}
using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;

namespace SheddingCardGame.UI.Pages
{
    public class LabelComponent : GameObject, IGameObject
    {
        public string Label { get; set; }
        public Point Position { get; }

        public LabelComponent(string label, Point position, bool isVisible)
        {
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
            await context.SetFontAsync("24px verdana");
            await context.SetStrokeStyleAsync("White");
            await context.SetFillStyleAsync("White");
            await context.FillTextAsync($"{Label}", Position.X, Position.Y);
        }

        public bool IsHit(Point mousePosition)
        {
            return false;
        }
    }
}
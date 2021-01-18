using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using SheddingCardGames;

namespace SheddingCardGame.UI.Pages
{
    public class CardComponent : GameObject, IGameObject
    {
        private readonly GameController gameController;
        public Card Card { get; }
        public Sprite Sprite { get; }

        private Rectangle boundingBox;
        private bool isActive = false;

        public CardComponent(GameController gameController, Card card, Sprite sprite, bool isVisible)
        {
            this.gameController = gameController;
            Card = card;
            Tag = $"{Card}";
            Sprite = sprite;
            boundingBox = new Rectangle(sprite.Position.X, sprite.Position.Y, sprite.Size.Width, sprite.Size.Height);
            IsVisible = isVisible;
        }

        public string Tag { get; set; }

        public bool IsHit(Point mousePosition)
        {
            return boundingBox.Contains(mousePosition);
        }

        public ValueTask Update(InputState inputState)
        {
            if (boundingBox.Contains(inputState.MouseCoords))
            {
                isActive = true;
                if (inputState.IsMouseDown)
                    gameController.CardClick(Card);
            }
            else
                isActive = false;

            return new ValueTask();
        }

        public async ValueTask Render(Canvas2DContext context)
        {
            var spriteX = Sprite.Position.X;
            var spriteY = Sprite.Position.Y;
            var frameWidth = Sprite.Size.Width;
            var frameHeight = Sprite.Size.Height;
            var frameCoords = GetFrameCoords(Card);
            var frameX = frameCoords.X * frameWidth;
            var frameY = frameCoords.Y * frameHeight;

            await context.DrawImageAsync(Sprite.SpriteSheet,
                frameX, frameY, frameWidth, frameHeight,
                spriteX, spriteY, frameWidth, frameHeight
            );

            if (isActive)
            {
                await context.BeginPathAsync();
                await context.SetStrokeStyleAsync($"rgb(255,0,0)");
                await context.SetLineWidthAsync(4);
                await context.StrokeRectAsync(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
            }
        }

        private (int X, int Y) GetFrameCoords(Card card)
        {
            if (card == null)
                return (2, 4);

            return (card.Rank - 1, (int)card.Suit);
        }
    }
}
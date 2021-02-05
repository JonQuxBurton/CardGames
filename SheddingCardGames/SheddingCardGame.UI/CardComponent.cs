using System.Drawing;
using System.Threading.Tasks;
using Blazor.Extensions.Canvas.Canvas2D;
using SheddingCardGames.Domain;

namespace SheddingCardGame.UI
{
    public class CardComponent : GameObject, IGameObject
    {
        private readonly Config config;
        public Card Card { get; }
        public Sprite Sprite { get; }
        public System.Action OnClick { get; set; }

        private Rectangle boundingBox;
        private bool isActive;

        public CardComponent(Config config, Card card, Sprite sprite, bool isVisible, bool isTurned = false)
        {
            this.config = config;
            Card = card;
            Tag = $"{Card}";
            IsTurnedUp = isTurned;
            Sprite = sprite;
            boundingBox = new Rectangle(sprite.Position.X, sprite.Position.Y, sprite.Size.Width, sprite.Size.Height);
            IsVisible = isVisible;
            SetX(sprite.Position.X);
            SetY(sprite.Position.Y);
        }

        public string Tag { get; set; }

        public new void SetX(int newX)
        {
            this.x = newX;
            Sprite.Position = new Point(newX, Sprite.Position.Y);
            boundingBox = new Rectangle(Sprite.Position.X, Sprite.Position.Y, Sprite.Size.Width, Sprite.Size.Height);
        }

        public new void SetY(int newY)
        {
            this.y = newY;
            Sprite.Position = new Point(Sprite.Position.X, newY);
            boundingBox = new Rectangle(Sprite.Position.X, Sprite.Position.Y, Sprite.Size.Width, Sprite.Size.Height);
        }

        public bool IsHit(Point mousePosition)
        {
            return boundingBox.Contains(mousePosition);
        }

        public ValueTask Update(InputState inputState)
        {
            if (boundingBox.Contains(inputState.MouseCoords))
            {
                isActive = true;
                if (inputState.IsMouseClicked)
                    OnClick?.Invoke();
            }
            else
                isActive = false;

            return new ValueTask();
        }

        public bool IsTurnedUp { get; set; }
        
        public async ValueTask Render(Canvas2DContext context)
        {
            var spriteX = GetX();
            var spriteY = GetY();
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
                await context.SetStrokeStyleAsync(config.HighlightColour);
                await context.SetLineWidthAsync(config.HighlightWidth);
                await context.StrokeRectAsync(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
            }
        }

        private (int X, int Y) GetFrameCoords(Card card)
        {
            if (!IsTurnedUp)
                return (2, 4);
            
            if (card == null)
                return (2, 4);

            return (card.Rank - 1, (int)card.Suit);
        }
    }
}
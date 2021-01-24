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

        public CardComponent(GameController gameController, Card card, Sprite sprite, bool isVisible, bool isTurned = false)
        {
            this.gameController = gameController;
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

        public int GetX()
        {
            return x;
        }
        
        public void SetX(int newX)
        {
            this.x = newX;
            Sprite.Position = new Point(newX, Sprite.Position.Y);
            boundingBox = new Rectangle(Sprite.Position.X, Sprite.Position.Y, Sprite.Size.Width, Sprite.Size.Height);
        }

        public int GetY()
        {
            return y;
        }

        public void SetY(int newY)
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
                    gameController.CardClick(this);
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
                await context.SetStrokeStyleAsync($"rgb(255,0,0)");
                await context.SetLineWidthAsync(4);
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
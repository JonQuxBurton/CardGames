using System;
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
        public System.Action OnRightClick { get; set; }

        private Rectangle boundingBox;
        private bool isActive;
        private bool isSelected;

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
                if (inputState.IsLeftMouseButtonClicked)
                    OnClick?.Invoke();
                else if (inputState.IsRightMouseButtonClicked)
                    OnRightClick?.Invoke();
                
                //isSelected = !isSelected;
            }
            else
                isActive = false;

            return new ValueTask();
        }

        public bool IsTurnedUp { get; set; }
        
        public async ValueTask Render(Canvas2DContext context)
        {
            var frameCoords = GetFrameCoords(Card);
            var frameX = frameCoords.X * config.SpriteSheetCardWidth;
            var frameY = frameCoords.Y * config.SpriteSheetCardHeight;

            var spriteX = GetX();
            var spriteY = GetY();
            var spriteWidth = Sprite.Size.Width;
            var spriteHeight = Sprite.Size.Height;

            await context.DrawImageAsync(Sprite.SpriteSheet,
                frameX, frameY, config.SpriteSheetCardWidth, config.SpriteSheetCardHeight,
                spriteX, spriteY, spriteWidth, spriteHeight
            );

            if (isActive)
            {
                await context.BeginPathAsync();
                await context.SetStrokeStyleAsync(config.HighlightColour);
                await context.SetLineWidthAsync(config.HighlightWidth);
                await context.StrokeRectAsync(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
            } 
            else if (isSelected)
            {
                await context.BeginPathAsync();
                await context.SetStrokeStyleAsync(config.SelectedColour);
                await context.SetLineWidthAsync(config.HighlightWidth);
                await context.StrokeRectAsync(boundingBox.X, boundingBox.Y, boundingBox.Width, boundingBox.Height);
            }
        }

        public void ToggleSelected()
        {
            isSelected = !isSelected;
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
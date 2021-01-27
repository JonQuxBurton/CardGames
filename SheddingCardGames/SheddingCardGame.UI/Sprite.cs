using System.Drawing;
using Microsoft.AspNetCore.Components;

namespace SheddingCardGame.UI
{
    public class Sprite
    {
        public ElementReference SpriteSheet { get; }
        public Size Size { get; }
        public Point Position { get; set; }

        public Sprite(ElementReference spriteSheet, Size size, Point position)
        {
            SpriteSheet = spriteSheet;
            Size = size;
            Position = position;
        }
    }
}
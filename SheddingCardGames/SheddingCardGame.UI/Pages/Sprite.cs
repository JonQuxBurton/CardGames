using System.Drawing;
using Microsoft.AspNetCore.Components;

namespace SheddingCardGame.UI.Pages
{
    public class Sprite
    {
        public ElementReference SpriteSheet { get; }
        public Size Size { get; }
        public Point Position { get; }

        public Sprite(ElementReference spriteSheet, Size size, Point position)
        {
            SpriteSheet = spriteSheet;
            Size = size;
            Position = position;
        }
    }
}
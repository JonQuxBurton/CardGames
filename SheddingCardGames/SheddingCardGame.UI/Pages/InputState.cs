using System.Drawing;

namespace SheddingCardGame.UI.Pages
{
    public class InputState
    {
        public bool IsMouseDown { get; }
        public Point MouseCoords { get; }

        public InputState(bool isMouseDown, Point mouseCoords)
        {
            IsMouseDown = isMouseDown;
            MouseCoords = mouseCoords;
        }
    }
}
using System.Drawing;

namespace SheddingCardGame.UI
{
    public class InputState
    {
        public bool IsLeftMouseButtonClicked { get; }
        public bool IsRightMouseButtonClicked { get; }
        public Point MouseCoords { get; }

        public InputState(bool isLeftMouseButtonClicked, bool isRightMouseButtonClicked, Point mouseCoords)
        {
            IsLeftMouseButtonClicked = isLeftMouseButtonClicked;
            IsRightMouseButtonClicked = isRightMouseButtonClicked;
            MouseCoords = mouseCoords;
        }
    }
}
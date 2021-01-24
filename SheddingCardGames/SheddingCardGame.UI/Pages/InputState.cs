﻿using System.Drawing;

namespace SheddingCardGame.UI.Pages
{
    public class InputState
    {
        public bool IsMouseClicked { get; }
        public Point MouseCoords { get; }

        public InputState(bool isMouseClicked, Point mouseCoords)
        {
            IsMouseClicked = isMouseClicked;
            MouseCoords = mouseCoords;
        }
    }
}
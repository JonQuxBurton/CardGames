using System;
using System.Drawing;

namespace SheddingCardGame.UI
{
    public class ButtonComponentBuilder
    {
        private readonly UiState uiState;

        public ButtonComponentBuilder(UiState uiState)
        {
            this.uiState = uiState;
        }

        public void Build(Cursor cursor, ButtonNames name, string label, Action buttonAction)
        {
            var component = new ButtonComponent(label, new Point(cursor.X, cursor.Y), true, buttonAction)
                {Tag = name.ToString()};
            uiState.GameObjects.Add(component);
            cursor.NextRow();
        }
    }
}
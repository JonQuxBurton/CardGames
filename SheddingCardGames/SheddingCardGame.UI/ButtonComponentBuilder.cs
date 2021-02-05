using System;
using System.Drawing;

namespace SheddingCardGame.UI
{
    public class ButtonComponentBuilder
    {
        private readonly Config config;
        private readonly UiState uiState;

        public ButtonComponentBuilder(Config config, UiState uiState)
        {
            this.config = config;
            this.uiState = uiState;
        }

        public void Build(Cursor cursor, ButtonNames name, string label, Action buttonAction)
        {
            var component = new ButtonComponent(config, label, new Point(cursor.X, cursor.Y), true, buttonAction)
                {Tag = name.ToString()};
            uiState.GameObjects.Add(component);
            cursor.NextRow();
        }
    }
}
using System.Drawing;

namespace SheddingCardGame.UI
{
    public class LabelComponentBuilder
    {
        private readonly UiState uiState;

        public LabelComponentBuilder(UiState uiState)
        {
            this.uiState = uiState;
        }

        public void Build(Cursor cursor, LabelNames name, string text = "")
        {
            var component = new LabelComponent(text, new Point(cursor.X, cursor.Y), true) {Tag = name.ToString()};
            uiState.GameObjects.Add(component);
            cursor.NextRow();
        }
    }
}
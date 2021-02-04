using System.Drawing;

namespace SheddingCardGame.UI
{
    public class LabelBuilder
    {
        private readonly UiState uiState;

        public LabelBuilder(UiState uiState)
        {
            this.uiState = uiState;
        }

        public void Build(Cursor cursor, LabelNames name)
        {
            var component = new LabelComponent("", new Point(cursor.X, cursor.Y), true) {Tag = name.ToString()};
            uiState.GameObjects.Add(component);
            cursor.NextRow();
        }
    }
}
using System.Drawing;

namespace SheddingCardGame.UI
{
    public class LabelComponentBuilder
    {
        private readonly Config config;
        private readonly UiState uiState;

        public LabelComponentBuilder(Config config, UiState uiState)
        {
            this.config = config;
            this.uiState = uiState;
        }

        public void Build(Cursor cursor, LabelNames name, string text = "")
        {
            var component = new LabelComponent(config, text, new Point(cursor.X, cursor.Y), true) {Tag = name.ToString()};
            uiState.GameObjects.Add(component);
            cursor.NextRow();
        }
    }
}
namespace SheddingCardGame.UI
{
    public class Cursor
    {
        private readonly int rowHeight;

        public Cursor(int x, int y, int rowHeight)
        {
            X = x;
            Y = y;
            this.rowHeight = rowHeight;
        }

        public int X { get; }
        public int Y { get; private set; }

        public void NextRow()
        {
            Y += rowHeight;
        }
    }
}
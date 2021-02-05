using System.Drawing;

namespace SheddingCardGame.UI
{
    public class Cursor
    {
        private readonly int rowHeight;

        public Cursor(Point point, int rowHeight = 0)
        {
            X = point.X;
            Y = point.Y;
            this.rowHeight = rowHeight;
        }

        public Cursor(int x, int y, int rowHeight = 0)
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
using System.Drawing;

namespace SheddingCardGame.UI
{
    public class Config
    {
        public int ScreenHeight { get; set; } = 800;
        public int ScreenWidth { get; set; } = 1200;
        public int CardHeight { get; set; } = 240;
        public int CardWidth { get; set; } = 154;

        public int LabelHeight { get; set; } = 35;

        public string TopPlayerLabel { get; set; } = "Player 1";
        public string BottomPlayerLabel { get; set; } = "Player 2";
        
        public string Font { get; set; } = "24px verdana";
        public string HighlightColour { get; set; } = "rgb(255,0,0)";
        public int HighlightWidth { get; set; } = 4;

        public Point ScreenCentre => new Point(ScreenWidth / 2, ScreenHeight / 2);

        public Rectangle TopPlayerInfoSection => new Rectangle(0, 10, ScreenWidth, LabelHeight);
        public Rectangle TopPlayerHandSection => new Rectangle(0, TopPlayerInfoSection.Bottom, ScreenWidth, CardHeight);
        public Rectangle TableSection => new Rectangle(0, TopPlayerHandSection.Bottom, ScreenWidth, CardHeight);
        public Rectangle BottomPlayerHandSection => new Rectangle(0, TableSection.Bottom, ScreenWidth, CardHeight);
        public Rectangle BottomPlayerInfoSection => new Rectangle(0, BottomPlayerHandSection.Bottom, ScreenWidth, LabelHeight);

        public Rectangle InfoSection => new Rectangle(360, TableSection.Y + LabelHeight, TableSection.Width - 360 - 500, TableSection.Height);
        public Rectangle ActionsSection => new Rectangle(InfoSection.Right, TableSection.Y + LabelHeight, TableSection.Width - 360 - 500, TableSection.Height);
    }
}
using System.Drawing;

namespace SheddingCardGame.UI
{
    public class Config
    {
        public int ScreenHeight { get; set; } = 800;
        public int ScreenWidth { get; set; } = 1200;
        public int SpriteSheetCardWidth { get; set; } = 154;
        public int SpriteSheetCardHeight { get; set; } = 240;
        public int CardWidth { get; set; } = 123;
        public int CardHeight { get; set; } = 192;

        public int LabelHeight { get; set; } = 35;

        public string Font { get; set; } = "24px verdana";
        public string FontColour { get; set; } = "White";
        public string BackgroundColour { get; set; } = "Green";
        public int ButtonBorderWidth { get; set; } = 2;
        public int ButtonPadding { get; set; } = 5;
        public string HighlightColour { get; set; } = "rgb(255,0,0)";
        public int HighlightWidth { get; set; } = 4;

        public Point ScreenCentre => new Point(ScreenWidth / 2, ScreenHeight / 2);

        public int Margin { get; set; } = 10;

        public Rectangle TopPlayerInfoSection => new Rectangle(Margin, Margin, ScreenWidth, LabelHeight);
        public Rectangle TopPlayerHandSection => new Rectangle(Margin, TopPlayerInfoSection.Bottom, ScreenWidth, CardHeight);
        public Rectangle TableSection => new Rectangle(Margin, TopPlayerHandSection.Bottom, ScreenWidth, CardHeight);
        public Rectangle BottomPlayerHandSection => new Rectangle(Margin, TableSection.Bottom, ScreenWidth, CardHeight);
        public Rectangle BottomPlayerInfoSection => new Rectangle(Margin, BottomPlayerHandSection.Bottom + Margin, ScreenWidth, LabelHeight);

        public Rectangle InfoSection => new Rectangle(360, TableSection.Y + Margin, TableSection.Width - 360 - 500, TableSection.Height);
        public Rectangle ActionsSection => new Rectangle(InfoSection.Right, TableSection.Y + LabelHeight, TableSection.Width - 360 - 500, TableSection.Height);

        public Rectangle StockPileSection => new Rectangle(TableSection.X, TableSection.Y, CardWidth, CardHeight);
        public Rectangle DiscardPileSection => new Rectangle(StockPileSection.Right, TableSection.Y, CardWidth, CardHeight);
    }
}
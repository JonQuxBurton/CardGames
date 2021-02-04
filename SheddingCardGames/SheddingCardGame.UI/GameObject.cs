namespace SheddingCardGame.UI
{
    public class GameObject
    {
        protected int x;
        protected int y;
        public bool IsVisible { get; set; } = true;
        private string Tag { get; set; }

        public int GetX()
        {
            return x;
        }

        public void SetX(int newX)
        {
            x = newX;
        }

        public int GetY()
        {
            return y;
        }

        public void SetY(int newY)
        {
            y = newY;
        }
    }
}
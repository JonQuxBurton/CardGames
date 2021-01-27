namespace SheddingCardGame.UI
{
    public class GameObject
    {
        public bool IsVisible { get; set; } = true;
        string Tag { get; set; }

        protected int x;
        protected int y;

        public int GetX()
        {
            return x;
        }

        public void SetX(int x)
        {
            this.x = x;
        }

        public int GetY()
        {
            return y;
        }

        public void SetY(int y)
        {
            this.y = y;
        }

    }
}
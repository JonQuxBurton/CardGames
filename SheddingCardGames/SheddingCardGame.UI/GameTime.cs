namespace SheddingCardGame.UI
{
    public class GameTime
    {
        private float totalTime = 0;

        /// <summary>
        /// total time elapsed since the beginning of the game
        /// </summary>
        public float TotalTime
        {
            get => totalTime;
            set
            {
                this.ElapsedTime = value - totalTime;
                totalTime = value;

            }
        }

        /// <summary>
        /// time elapsed since last frame
        /// </summary>
        public float ElapsedTime { get; private set; }
    }
}

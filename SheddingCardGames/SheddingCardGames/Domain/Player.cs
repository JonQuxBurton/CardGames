namespace SheddingCardGames.Domain
{
    public class Player
    {
        public int Number { get; }

        public Player(int number)
        {
            Number = number;
        }

        public CardCollection Hand { get; set; } = new CardCollection();
    }
}
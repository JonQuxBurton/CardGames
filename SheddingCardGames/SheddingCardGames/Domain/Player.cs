namespace SheddingCardGames.Domain
{
    public class Player
    {
        public int Number { get; }
        public string Name { get; }

        public Player(int number, string name)
        {
            Number = number;
            Name = name;
        }

        public CardCollection Hand { get; set; } = new CardCollection();
    }
}
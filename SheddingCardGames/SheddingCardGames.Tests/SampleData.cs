using System.Collections.Generic;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests
{
    public class SampleData
    {
        public Player Player1;
        public Player Player2;
        public Player Player3;
        public Player Player4;

        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();

        public Player GetPlayer(int number)
        {
            return players[number];
        }

        public SampleData()
        {
            Player1 = new Player(1, "Alice");
            Player2 = new Player(2, "Bob");
            Player3 = new Player(3, "Carol");
            Player4 = new Player(4, "Dan");

            players.Add(1, Player1);
            players.Add(2, Player2);
            players.Add(3, Player3);
            players.Add(4, Player4);
        }
    }
}

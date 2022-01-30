using CardGamesDomain;

namespace RummyGames
{
    public class Turn
    {
        public Turn(int number, Player currentPlayer, Card takenCard = null)
        {
            Number = number;
            CurrentPlayer = currentPlayer;
            TakenCard = takenCard;
        }

        public int Number { get; }
        public Player CurrentPlayer { get; }
        public Card TakenCard { get; }
    }
}
namespace RummyGames
{
    public class StartingPlayerChooser : IStartingPlayerChooser
    {
        public Player Choose(Player player1, Player player2)
        {
            return player1;
        }
    }
}
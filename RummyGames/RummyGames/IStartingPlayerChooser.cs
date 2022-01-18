namespace RummyGames
{
    public interface IStartingPlayerChooser
    {
        Player Choose(Player player1, Player player2);
    }
}
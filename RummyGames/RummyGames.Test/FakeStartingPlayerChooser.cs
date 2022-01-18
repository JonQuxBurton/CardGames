namespace RummyGames.Test
{
    public class FakeStartingPlayerChooser : IStartingPlayerChooser
    {
        private readonly Player playerToChoose;

        public FakeStartingPlayerChooser(Player playerToChoose)
        {
            this.playerToChoose = playerToChoose;
        }

        public Player Choose(Player player1, Player player2)
        {
            return player2.Id == playerToChoose.Id ? player2 : player1;
        }
    }
}
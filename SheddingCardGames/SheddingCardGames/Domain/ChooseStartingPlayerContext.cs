namespace SheddingCardGames.Domain
{
    public class ChooseStartingPlayerContext : ICommandContext
    {
        public ChooseStartingPlayerContext(Player chosenPlayer)
        {
            ChosenPlayer = chosenPlayer;
        }

        public Player ChosenPlayer { get; }
    }
}
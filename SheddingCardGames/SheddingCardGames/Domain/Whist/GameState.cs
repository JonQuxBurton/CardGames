using SheddingCardGames.Domain.CrazyEights;

namespace SheddingCardGames.Domain.Whist
{
    public class GameState
    {
        public GameState()
        {
            EventLog = new EventLog();
        }

        public GameSetup GameSetup { get; set; }
        public Table CurrentTable { get; set; }
        public StateOfPlay CurrentStateOfPlay { get; set; }
        public StateOfTrick CurrentStateOfTrick { get; set; }
        public StateOfTurn PreviousStateOfTurn { get; set; }
        public EventLog EventLog { get; }

        public Player NextPlayer
        {
            get
            {
                var nextPlayerNumber = CurrentStateOfTrick.PlayerToPlay.Number + 1;
                if (nextPlayerNumber > CurrentTable.Players.Count)
                    nextPlayerNumber = 1;

                return CurrentTable.Players[nextPlayerNumber - 1];
            }
        }
        
        public int CurrentTrickNumber => CurrentStateOfTrick.TrickNumber;
        public Player CurrentPlayerToPlay => CurrentStateOfTrick.PlayerToPlay;
        public int CurrentPlayerToPlayNumber => CurrentStateOfTrick.PlayerToPlay.Number;
    }
}
using System.Linq;

namespace SheddingCardGames.Domain
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
        public StateOfTurn CurrentStateOfTurn { get; set; }
        public StateOfTurn PreviousStateOfTurn { get; set; }
        public EventLog EventLog { get; }

        public Player NextPlayer
        {
            get
            {
                var nextPlayerNumber = CurrentStateOfTurn.PlayerToPlay.Number + 1;
                if (nextPlayerNumber > CurrentTable.Players.Count)
                    nextPlayerNumber = 1;

                return CurrentTable.Players[nextPlayerNumber - 1];
            }
        }

        public bool AnyPlaysOrTakes
        {
            get
            {
                if (CurrentStateOfTurn == null)
                    return false;

                return CurrentStateOfTurn.TurnNumber != 1 ||
                       CurrentStateOfTurn.PreviousActions.Any();
            }
        }

        public int CurrentTurnNumber => CurrentStateOfTurn.TurnNumber;
        public Player CurrentPlayerToPlay => CurrentStateOfTurn.PlayerToPlay;
        public int CurrentPlayerToPlayNumber => CurrentStateOfTurn.PlayerToPlay.Number;
        public Card CurrentCardToMatch => CurrentTable.DiscardPile.CardToMatch;
        public Suit? CurrentSelectedSuit => CurrentStateOfTurn?.SelectedSuit;
    }
}
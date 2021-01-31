namespace SheddingCardGames.UiLogic
{
    public class GameState
    {
        public GamePhase CurrentGamePhase { get;  }

        public GameState(GamePhase currentGamePhase)
        {
            CurrentGamePhase = currentGamePhase;
        }
    }
}
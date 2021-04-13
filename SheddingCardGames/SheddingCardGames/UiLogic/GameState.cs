using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class GameState
    {
        public GamePhase CurrentGamePhase { get;  }
        public Table CurrentTable { get; }
        public int? StartingPlayer { get; }
        public CurrentTurn CurrentTurn { get; }

        public GameState(GamePhase currentGamePhase, int? startingPlayer = null, Table currentTable = null, CurrentTurn currentTurn = null)
        {
            CurrentGamePhase = currentGamePhase;
            CurrentTable = currentTable;
            CurrentTurn = currentTurn;
            StartingPlayer = startingPlayer;
        }

        public GameState WithGamePhase(GamePhase newGamePhase)
        {
            return new GameState(newGamePhase, StartingPlayer, CurrentTable, CurrentTurn);
        }

        public GameState WithTable(Table newTable)
        {
            return new GameState(CurrentGamePhase, StartingPlayer, newTable, CurrentTurn);
        }
        
        public GameState WithCurrentTurn(CurrentTurn newCurrentTurn)
        {
            return new GameState(CurrentGamePhase, StartingPlayer, CurrentTable, newCurrentTurn);
        }
    }
}
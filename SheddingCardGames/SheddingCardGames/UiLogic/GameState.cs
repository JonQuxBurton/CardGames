using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class GameState
    {
        public GamePhase CurrentGamePhase { get;  }
        public Board CurrentBoard { get; }
        public int? StartingPlayer { get; }
        public Turn CurrentTurn { get; }

        public GameState(GamePhase currentGamePhase, int? startingPlayer = null, Board currentBoard = null, Turn currentTurn = null)
        {
            CurrentGamePhase = currentGamePhase;
            CurrentBoard = currentBoard;
            CurrentTurn = currentTurn;
            StartingPlayer = startingPlayer;
        }

        public GameState WithGamePhase(GamePhase newGamePhase)
        {
            return new GameState(newGamePhase, StartingPlayer, CurrentBoard, CurrentTurn);
        }

        public GameState WithBoard(Board newBoard)
        {
            return new GameState(CurrentGamePhase, StartingPlayer, newBoard, CurrentTurn);
        }
        
        public GameState WithCurrentTurn(Turn newCurrentTurn)
        {
            return new GameState(CurrentGamePhase, StartingPlayer, CurrentBoard, newCurrentTurn);
        }
    }
}
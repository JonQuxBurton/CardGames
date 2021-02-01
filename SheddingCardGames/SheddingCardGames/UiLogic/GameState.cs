using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class GameState
    {
        public GamePhase CurrentGamePhase { get;  }
        public Board CurrentBoard { get; }
        public int? StartingPlayer { get; }
        public Turn CurrentTurn { get; set; }

        public GameState(GamePhase currentGamePhase, int? startingPlayer = null, Board currentBoard = null, Turn currentTurn = null)
        {
            CurrentGamePhase = currentGamePhase;
            CurrentBoard = currentBoard;
            CurrentTurn = currentTurn;
            StartingPlayer = startingPlayer;
        }

        public static GameState WithBoard(GameState original, Board newBoard)
        {
            return new GameState(original.CurrentGamePhase, original.StartingPlayer, newBoard, original.CurrentTurn);
        }
    }
}
using System.Linq;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class StateOfPlay
    {
        private readonly GameState gameState;

        public StateOfPlay(GameState gameState, GamePhase gamePhase = GamePhase.New, Player winner = null)
        {
            this.gameState = gameState;
            Winner = winner;
            CurrentGamePhase = gamePhase;
        }

        public GamePhase CurrentGamePhase { get; }
        public bool HasWinner => Winner != null;
        public Player Winner { get; }

        public bool AnyPlaysOrTakes
        {
            get
            {
                if (gameState.CurrentStateOfTurn == null)
                    return false;

                return gameState.CurrentStateOfTurn.TurnNumber != 1 ||
                       gameState.CurrentStateOfTurn.PreviousActions.Any();
            }
        }

        public static StateOfPlay WithWinner(StateOfPlay current, Player winner)
        {
            return new StateOfPlay(current.gameState, current.CurrentGamePhase, winner);
        }
        
        public static StateOfPlay WithGamePhaseReadyToDeal(StateOfPlay current)
        {
            return new StateOfPlay(current.gameState, GamePhase.ReadyToDeal, current.Winner);
        }
        
        public static StateOfPlay WithGamePhaseInGame(StateOfPlay current)
        {
            return new StateOfPlay(current.gameState, GamePhase.InGame, current.Winner);
        }
    }
}
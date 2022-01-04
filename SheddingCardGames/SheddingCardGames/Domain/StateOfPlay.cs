using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class StateOfPlay
    {
        public StateOfPlay(GamePhase gamePhase = GamePhase.New, Player winner = null)
        {
            Winner = winner;
            CurrentGamePhase = gamePhase;
        }

        public GamePhase CurrentGamePhase { get; }
        public bool HasWinner => Winner != null;
        public Player Winner { get; }

        public static StateOfPlay WithWinner(StateOfPlay current, Player winner)
        {
            return new StateOfPlay(current.CurrentGamePhase, winner);
        }

        public static StateOfPlay WithGamePhaseReadyToDeal(StateOfPlay current)
        {
            return new StateOfPlay(GamePhase.ReadyToDeal, current.Winner);
        }

        public static StateOfPlay WithGamePhaseInGame(StateOfPlay current)
        {
            return new StateOfPlay(GamePhase.InGame, current.Winner);
        }
    }
}
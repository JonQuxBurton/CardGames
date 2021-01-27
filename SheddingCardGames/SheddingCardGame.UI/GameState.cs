namespace SheddingCardGame.UI
{
    public class GameState
    {
        public GamePhase CurrentGamePhase = GamePhase.New;
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
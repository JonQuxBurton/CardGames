namespace SheddingCardGames.Domain
{
    public class PreviousTurnResult
    {
        public PreviousTurnResult(bool hasWinner, Player winner = null, Suit? selectedSuit = null, Card takenCard = null)
        {
            HasWinner = hasWinner;
            Winner = winner;
            SelectedSuit = selectedSuit;
            TakenCard = takenCard;
        }

        public bool HasWinner { get; }
        public Player Winner { get; }
        public Suit? SelectedSuit { get; set; }
        public Card TakenCard { get; set; }
    }
}
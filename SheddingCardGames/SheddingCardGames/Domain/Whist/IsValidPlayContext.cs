namespace SheddingCardGames.Domain.Whist
{
    public class IsValidPlayContext
    {
        public IsValidPlayContext(Card cardPlayed, StateOfTrick currentStateOfTrick)
        {
            CardPlayed = cardPlayed;
            CurrentStateOfTrick = currentStateOfTrick;
        }


        public Card CardPlayed { get; }
        public StateOfTrick CurrentStateOfTrick { get; }
    }
}
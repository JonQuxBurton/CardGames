namespace SheddingCardGames.Domain.Whist
{
    public class IsValidPlayContext
    {
        public IsValidPlayContext(Card cardPlayed, CardCollection playerHand, StateOfTrick currentStateOfTrick)
        {
            CardPlayed = cardPlayed;
            PlayerHand = playerHand;
            CurrentStateOfTrick = currentStateOfTrick;
        }


        public Card CardPlayed { get; }
        public CardCollection PlayerHand { get; }
        public StateOfTrick CurrentStateOfTrick { get; }
    }
}
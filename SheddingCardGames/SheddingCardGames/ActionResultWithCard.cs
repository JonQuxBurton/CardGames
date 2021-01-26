namespace SheddingCardGames
{
    public class ActionResultWithCard
    {
        public ActionResultWithCard(bool isSuccess, ActionResultMessageKey messageKey, Card card = null)
        {
            IsSuccess = isSuccess;
            MessageKey = messageKey;
            Card = card;
        }

        public bool IsSuccess { get; }
        public ActionResultMessageKey MessageKey { get; }

        public Card Card { get; }
        
        public override string ToString()
        {
            return $"{IsSuccess}[{MessageKey}] Card: {Card}";
        }
    }
}
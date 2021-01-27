namespace SheddingCardGames.Domain
{
    public class ActionResult
    {
        public ActionResult(bool isSuccess, ActionResultMessageKey messageKey)
        {
            IsSuccess = isSuccess;
            MessageKey = messageKey;
        }

        public bool IsSuccess { get; }
        public ActionResultMessageKey MessageKey { get; }

        public override string ToString()
        {
            return $"{IsSuccess}[{MessageKey}]";
        }
    }
}
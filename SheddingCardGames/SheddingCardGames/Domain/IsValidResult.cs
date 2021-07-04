namespace SheddingCardGames.Domain
{
    public class IsValidResult
    {
        public IsValidResult(bool isValid, CommandIsValidResultMessageKey messageKey)
        {
            IsValid = isValid;
            MessageKey = messageKey;
        }

        public bool IsValid { get; }
        public CommandIsValidResultMessageKey MessageKey { get; }

        public override string ToString()
        {
            return $"{IsValid}[{MessageKey}]";
        }
    }
}
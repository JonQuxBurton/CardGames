namespace SheddingCardGames.Domain
{
    public class IsValidResult
    {
        public IsValidResult(bool isValid, CommandExecutionResultMessageKey messageKey)
        {
            IsValid = isValid;
            MessageKey = messageKey;
        }

        public bool IsValid { get; }
        public CommandExecutionResultMessageKey MessageKey { get; }

        public override string ToString()
        {
            return $"{IsValid}[{MessageKey}]";
        }
    }
}
namespace SheddingCardGames.Domain
{
    public class CommandExecutionResult
    {
        public CommandExecutionResult(bool isSuccess, CommandIsValidResultMessageKey messageKey)
        {
            IsSuccess = isSuccess;
            MessageKey = messageKey;
        }

        public bool IsSuccess { get; }
        public CommandIsValidResultMessageKey MessageKey { get; }

        public override string ToString()
        {
            return $"{IsSuccess}[{MessageKey}]";
        }
    }
}
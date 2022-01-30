namespace RummyGames
{
    public class Result
    {
        public Result(bool isSuccess, ErrorKey errorKey, InGameState newInGameState)
        {
            IsSuccess = isSuccess;
            ErrorKey = errorKey;
            NewInGameState = newInGameState;
        }

        public bool IsSuccess { get; }
        public ErrorKey ErrorKey { get; }
        public InGameState NewInGameState { get; }
    }
}
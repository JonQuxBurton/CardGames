namespace SheddingCardGames.Domain
{
    public enum CommandIsValidResultMessageKey
    {
        Success,
        CardIsNotInPlayersHand,
        NotPlayersTurn,
        InvalidPlay,
        InvalidTake,
        GameCompleted
    }
}
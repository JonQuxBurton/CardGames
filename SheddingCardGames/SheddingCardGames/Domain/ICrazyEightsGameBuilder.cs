namespace SheddingCardGames.Domain
{
    public interface ICrazyEightsGameBuilder
    {
        Game Build(CardCollection deck, int numberOfPlayers);
    }
}
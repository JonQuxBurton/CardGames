namespace SheddingCardGames.Domain
{
    public interface ICrazyEightsGameBuilder
    {
        Game Build(IShuffler shuffler, VariantName variantNameName, int numberOfPlayers);
    }
}
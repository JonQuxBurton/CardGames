namespace SheddingCardGames.Domain.CrazyEights
{
    public interface ICrazyEightsGameBuilder
    {
        Game Build(IShuffler shuffler, VariantName variantNameName, int numberOfPlayers);
    }
}
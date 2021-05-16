namespace SheddingCardGames.Domain
{
    public interface ICrazyEightsGameBuilder
    {
        Game Build(VariantName variantNameName, int numberOfPlayers);
    }
}
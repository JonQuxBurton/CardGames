namespace SheddingCardGames.Domain
{
    public interface ICrazyEightsGameBuilder
    {
        Game Build(VariantName variantNameName, CardCollection deck, int numberOfPlayers);
    }
}
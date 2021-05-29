namespace SheddingCardGames.Domain
{
    public class OlsenOlsenVariantRules : CrazyEightsRules
    {
        public OlsenOlsenVariantRules(NumberOfPlayers numberOfPlayers) : base(numberOfPlayers)
        {
        }

        public override int NumberOfTakesBeforePass { get; } = 3;
    }
}
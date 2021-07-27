namespace SheddingCardGames.Domain.CrazyEights
{
    public class BasicVariantRules : CrazyEightsRules
    {
        public BasicVariantRules(NumberOfPlayers numberOfPlayers) : base(numberOfPlayers)
        {
        }

        public override int NumberOfTakesBeforePass { get; } = 1;
    }
}
namespace SheddingCardGames.Domain.CrazyEights
{
    public class Variant
    {
        public Variant(VariantName name, ICommandFactory commandFactory)
        {
            Name = name;
            CommandFactory = commandFactory;
        }

        public VariantName Name { get; }
        public ICommandFactory CommandFactory { get; }
    }
}
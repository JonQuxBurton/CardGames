namespace SheddingCardGames.Domain
{
    public enum VariantName
    {
        Basic,
        OlsenOlsen
    }

    public class Variant
    {
        public Variant(ICommandFactory commandFactory)
        {
            CommandFactory = commandFactory;
        }

        public ICommandFactory CommandFactory { get; }
    }
}
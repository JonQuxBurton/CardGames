using SheddingCardGames.Domain.CrazyEights;

namespace SheddingCardGames.Domain.Whist
{
    public class WhistClassicVariantCommandFactory : ICommandFactory
    {
        private readonly IRandomPlayerChooser randomPlayerChooser;
        private readonly WhistConfiguration whistConfiguration;
        private readonly IShuffler shuffler;

        public WhistClassicVariantCommandFactory(WhistConfiguration whistConfiguration, IShuffler shuffler, IRandomPlayerChooser randomPlayerChooser)
        {
            this.whistConfiguration = whistConfiguration;
            this.shuffler = shuffler;
            this.randomPlayerChooser = randomPlayerChooser;
        }

        public GameCommand Create(GameState gameState, ICommandContext context)
        {
            var rules = new Rules();

            if (context is ChooseStartingPlayerContext chooseStartingPlayerContext)
                return new ChooseStartingPlayerCommand(randomPlayerChooser, gameState, chooseStartingPlayerContext);

            if (context is DealContext dealContext)
                return new DealCommand(whistConfiguration, shuffler, gameState, dealContext);

            if (context is PlayContext playContext)
                return new PlayCommand(rules, gameState, playContext);

            return null;
        }
    }
}
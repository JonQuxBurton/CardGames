using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class OlsenOlsenVariantCommandFactory : ICommandFactory
    {
        private readonly CrazyEightsRules crazyEightsRules;
        private readonly IShuffler shuffler;

        public OlsenOlsenVariantCommandFactory(CrazyEightsRules crazyEightsRules, IShuffler shuffler)
        {
            this.crazyEightsRules = crazyEightsRules;
            this.shuffler = shuffler;
        }

        public GameCommand Create(GameState gameState, ICommandContext context)
        {
            if (context is ChooseStartingPlayerContext chooseStartingPlayerContext)
                return new ChooseStartingPlayerCommand(gameState, chooseStartingPlayerContext);

            if (context is DealContext dealContext)
                return new DealCommand(crazyEightsRules, shuffler, gameState, dealContext);

            if (context is PlayContext playContext)
                return new PlayCommand(crazyEightsRules, gameState, playContext);

            if (context is SelectSuitContext selectSuitContext)
                return new SelectSuitCommand(crazyEightsRules, gameState, selectSuitContext);

            if (context is TakeContext takeContext)
                return new TakeCommand(crazyEightsRules, shuffler, gameState, takeContext);

            return null;
        }
    }
}
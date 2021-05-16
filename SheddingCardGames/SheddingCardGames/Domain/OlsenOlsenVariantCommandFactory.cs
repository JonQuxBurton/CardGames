using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class OlsenOlsenVariantCommandFactory : ICommandFactory
    {
        private readonly IRules rules;
        private readonly IShuffler shuffler;

        public OlsenOlsenVariantCommandFactory(IRules rules, IShuffler shuffler)
        {
            this.rules = rules;
            this.shuffler = shuffler;
        }

        public GameCommand Create(GameState gameState, ICommandContext context)
        {
            if (context is ChooseStartingPlayerContext chooseStartingPlayerContext)
                return new ChooseStartingPlayerCommand(gameState, chooseStartingPlayerContext);

            if (context is DealContext dealContext)
                return new DealCommand(rules, shuffler, gameState, dealContext);

            if (context is PlayContext playContext)
                return new PlayCommand(rules, gameState, playContext);

            if (context is SelectSuitContext selectSuitContext)
                return new SelectSuitCommand(rules, gameState, selectSuitContext);

            if (context is TakeContext takeContext)
                return new TakeCommand(rules, shuffler, gameState, takeContext);

            return null;
        }
    }
}
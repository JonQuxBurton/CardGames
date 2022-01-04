using System.Collections.Generic;
using static SheddingCardGames.Domain.PlayersUtils;

namespace SheddingCardGames.Domain.Whist
{
    public class WhistClassic
    {
        public CardCollection Deck { get; }
        public readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();
        public ICommandFactory CommandFactory { get; }

        public WhistClassic(Player[] withPlayers, WhistConfiguration whistConfiguration, IShuffler shuffler,
            IRandomPlayerChooser randomPlayerChooser, CardCollection deck)
        {
            Deck = deck;
            CommandFactory = new WhistClassicVariantCommandFactory(whistConfiguration, shuffler, randomPlayerChooser);

            foreach (var player in withPlayers)
                Players.Add(player.Number, player);

            GameState = new GameState
            {
                GameSetup = new GameSetup(Players(withPlayers)),
                CurrentStateOfPlay = new StateOfPlay()
            };
        }

        public GameState GameState { get; private set; }

        public Player GetPlayer(int playerNumber)
        {
            return Players[playerNumber];
        }

        public void Initialise(GameState initialGameState)
        {
            GameState = initialGameState;
        }

        public CommandExecutionResult ChooseStartingPlayer(ChooseStartingPlayerContext chooseStartingPlayerContext)
        {
            return ProcessCommand(chooseStartingPlayerContext);
        }

        public CommandExecutionResult Deal(DealContext dealContext)
        {
            return ProcessCommand(dealContext);
        }

        public CommandExecutionResult Play(PlayContext playContext)
        {
            return ProcessCommand(playContext);
        }

        private CommandExecutionResult ProcessCommand(ICommandContext commandContext)
        {
            var command = CommandFactory.Create(GameState, commandContext);

            var isValidResult = command.IsValid();

            if (!isValidResult.IsValid)
                return new CommandExecutionResult(isValidResult.IsValid, isValidResult.MessageKey);

            GameState = command.Execute();

            return new CommandExecutionResult(isValidResult.IsValid, isValidResult.MessageKey);
        }
    }
}
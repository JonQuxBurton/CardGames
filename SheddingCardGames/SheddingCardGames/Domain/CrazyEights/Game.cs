using System.Collections.Generic;
using static SheddingCardGames.Domain.PlayersUtils;

namespace SheddingCardGames.Domain.CrazyEights
{
    public class Game
    {
        public readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();

        public Game(Variant variant, Player[] withPlayers)
        {
            Variant = variant;

            foreach (var player in withPlayers)
                Players.Add(player.Number, player);

            GameState = new GameState
            {
                GameSetup = new GameSetup(Players(withPlayers)),
                CurrentStateOfPlay = new StateOfPlay()
            };
        }

        public Variant Variant { get; }


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

        public CommandExecutionResult SelectSuit(SelectSuitContext selectSuitContext)
        {
            return ProcessCommand(selectSuitContext);
        }

        public CommandExecutionResult Take(TakeContext takeContext)
        {
            return ProcessCommand(takeContext);
        }

        private CommandExecutionResult ProcessCommand(ICommandContext commandContext)
        {
            var command = Variant.CommandFactory.Create(GameState, commandContext);

            var isValidResult = command.IsValid();

            if (!isValidResult.IsValid)
                return new CommandExecutionResult(isValidResult.IsValid, isValidResult.MessageKey);

            GameState = command.Execute();

            return new CommandExecutionResult(isValidResult.IsValid, isValidResult.MessageKey);
        }
    }
}
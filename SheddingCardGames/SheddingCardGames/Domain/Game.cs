using System.Collections.Generic;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.PlayersUtils;

namespace SheddingCardGames.Domain
{
    public class Game
    {
        public readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();

        public Game(Variant variant, Player[] withPlayers)
        {
            Variant = variant;

            foreach (var player in withPlayers)
                Players.Add(player.Number, player);

            GameState = new GameState(Players(withPlayers));
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

        public ActionResult ChooseStartingPlayer(ChooseStartingPlayerContext chooseStartingPlayerContext)
        {
            return ProcessCommand(chooseStartingPlayerContext);
        }

        public ActionResult Deal(DealContext dealContext)
        {
            return ProcessCommand(dealContext);
        }

        public ActionResult Play(PlayContext playContext)
        {
            return ProcessCommand(playContext);
        }

        public ActionResult SelectSuit(SelectSuitContext selectSuitContext)
        {
            return ProcessCommand(selectSuitContext);
        }

        public ActionResult Take(TakeContext takeContext)
        {
            return ProcessCommand(takeContext);
        }

        private ActionResult ProcessCommand(ICommandContext commandContext)
        {
            var command = Variant.CommandFactory.Create(GameState, commandContext);

            var isValidResult = command.IsValid();

            if (!isValidResult.IsSuccess)
                return isValidResult;

            GameState = command.Execute();

            return isValidResult;
        }
    }
}
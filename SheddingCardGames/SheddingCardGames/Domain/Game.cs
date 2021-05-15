using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class Game
    {
        private readonly CardCollection deck;

        public readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();

        public Game(Variant variant, CardCollection deck, Player[] withPlayers)
        {
            Variant = variant;
            this.deck = deck;

            foreach (var player in withPlayers)
                Players.Add(player.Number, player);

            GameState = new GameState();
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

        public ActionResult Deal()
        {
            return ProcessCommand(new DealContext(deck, Players.Values.ToArray()));
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
using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class Game
    {
        private readonly Dictionary<string, Card> cards = new Dictionary<string, Card>();
        private readonly Variant variant;
        private readonly CardCollection deck;

        public readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();

        public Game(Variant variant, CardCollection deck, Player[] withPlayers)
        {
            this.variant = variant;
            this.deck = deck;

            foreach (var card in deck.Cards)
                if (!cards.ContainsKey($"{card.Rank}|{card.Suit}"))
                    cards.Add($"{card.Rank}|{card.Suit}", card);

            foreach (var player in withPlayers)
                Players.Add(player.Number, player);

            GameState = new GameState();
            
        }

        public GameState GameState { get; private set; }

        public Player GetPlayer(int playerNumber)
        {
            return Players[playerNumber];
        }

        public Card GetCard(int rank, Suit suit)
        {
            var key = $"{rank}|{suit}";
            if (cards.ContainsKey(key))
                return cards[key];

            return null;
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
            var command = variant.CommandFactory.Create(GameState, commandContext);

            var isValidResult = command.IsValid();

            if (!isValidResult.IsSuccess)
                return isValidResult;

            GameState = command.Execute();

            return isValidResult;
        }
    }
}
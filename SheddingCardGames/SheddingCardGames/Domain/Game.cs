using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class Game
    {
        private readonly Dictionary<string, Card> cards = new Dictionary<string, Card>();
        private readonly CardCollection deck;

        public readonly Dictionary<int, Player> Players = new Dictionary<int, Player>();
        private readonly IRules rules;
        private readonly IShuffler shuffler;

        public Game(IRules rules, IShuffler shuffler, CardCollection deck,
            Player[] withPlayers)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.deck = deck;

            foreach (var card in deck.Cards)
                if (!cards.ContainsKey($"{card.Rank}|{card.Suit}"))
                    cards.Add($"{card.Rank}|{card.Suit}", card);

            foreach (var player in withPlayers)
                Players.Add(player.Number, player);

            GameState = new GameState();
        }

        public GameState GameState { get; private set; }

        public Player CurrentPlayer => GameState.PlayerToPlay;

        public Card GetCard(int rank, Suit suit)
        {
            var key = $"{rank}|{suit}";
            if (cards.ContainsKey(key))
                return cards[key];

            return null;
        }

        public void Initialise2(GameState initialGameState)
        {
            GameState = initialGameState;
        }

        //public void Initialise(GameState initialGameState)
        //{
        //    GameState = initialGameState;
        //    //events.Add(new Initialised(1));
        //}

        public void ChooseStartingPlayer(Player chosenPlayer)
        {
            GameCommand command = new ChooseStartingPlayerCommand(chosenPlayer);
            GameState = command.Execute();
            GameState.CurrentGamePhase = GamePhase.ReadyToDeal;
        }

        public void Deal()
        {
            GameCommand command = new DealCommand(shuffler, rules, GameState, deck, Players.Values.ToArray());

            var updatedGameState = command.Execute();
            GameState = updatedGameState;
            GameState.CurrentGamePhase = GamePhase.InGame;
        }

        public ActionResult Play(int playerNumber, Card playedCard)
        {
            var context = new PlayCommandContext(Players[playerNumber], playedCard);
            GameCommand command = new PlayCommand(GameState.PlayerToPlay, rules, GameState, context);

            var isValidResult = command.IsValid();

            if (!isValidResult.IsSuccess)
                return isValidResult;

            var updatedGameState = command.Execute();
            GameState = updatedGameState;

            return isValidResult;
        }

        public ActionResult SelectSuit(int playerNumber, Suit selectedSuit)
        {
            var context = new SelectSuitCommandContext(selectedSuit, Players[playerNumber]);
            GameCommand command = new SelectSuitCommand(rules, CurrentPlayer, GameState, context);

            var isValidResult = command.IsValid();

            if (!isValidResult.IsSuccess)
                return isValidResult;

            var updatedGameState = command.Execute();
            GameState = updatedGameState;

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public ActionResultWithCard Take(int playerNumber)
        {
            var context = new TakeCommandContext(Players[playerNumber]);
            GameCommand command = new TakeCommand(rules, shuffler, GameState, context);

            var isValidResult = command.IsValid();

            if (!isValidResult.IsSuccess)
                return new ActionResultWithCard(false, isValidResult.MessageKey);

            var updatedGameState = command.Execute();
            GameState = updatedGameState;

            return new ActionResultWithCard(true, ActionResultMessageKey.Success, updatedGameState.TakenCard);
        }
    }
}
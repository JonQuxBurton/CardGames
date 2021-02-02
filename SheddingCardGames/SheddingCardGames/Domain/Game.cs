using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class Game
    {
        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        private readonly IRules rules;
        private readonly IShuffler shuffler;
        private readonly IDealer dealer;
        private readonly List<Turn> turns;

        public Game(IRules rules, IShuffler shuffler, IDealer dealer, IEnumerable<Player> withPlayers)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.dealer = dealer;

            foreach (var player in withPlayers)
                players.Add(player.Number, player);

            turns = new List<Turn>();
            GameState = new GameState(GamePhase.New);
        }

        public GameState GameState { get; private set; }
        public IEnumerable<Turn> Turns => turns;

        public IEnumerable<CardMoveEvent> CardMoves => GameState.CurrentBoard.CardMoves;

        public Turn GetCurrentTurn()
        {
            return !turns.Any() ? null : turns.Last();
        }

        public void Initialise(GameState initialGameState)
        {
            GameState = initialGameState;
            
            turns.Add(initialGameState.CurrentTurn);
        }
        
        public void ChooseStartingPlayer(int chosenPlayer)
        {
            GameState = new GameState(GamePhase.ReadyToDeal, chosenPlayer);
        }

        public void Deal()
        {
            GameState = new GameState(GamePhase.InGame, GameState.StartingPlayer);
            GameState = GameState.WithBoard(GameState, dealer.Deal(players.Values));
            AddFirstTurn(players[GameState.StartingPlayer.Value]);

            var winner = GetWinner();
            if (winner != null)
                AddWinningTurn();
        }
        
        public ActionResult Play(int playerNumber, Card playedCard)
        {
            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (CurrentPlayer.Number != playerNumber)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!CurrentPlayer.Hand.Contains(playedCard))
                return new ActionResult(false, ActionResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay(playedCard))
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            GameState.CurrentBoard.MoveCardToDiscardPile(CurrentPlayer, playedCard);

            if (IsWinner())
                AddWinningTurn();
            else if (playedCard.Rank == 8)
                AddCrazyEightTurn();
            else
                AddNextTurn(NextPlayer);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public ActionResult SelectSuit(int playerNumber, Suit selectedSuit)
        {
            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (CurrentPlayer.Number != playerNumber)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);
            
            AddNextTurn(NextPlayer, selectedSuit);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public ActionResultWithCard Take(int playerNumber)
        {
            if (GetCurrentTurn().NextAction != Action.Take)
                return new ActionResultWithCard(false, ActionResultMessageKey.InvalidTake);

            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);

            if (CurrentPlayer.Number != playerNumber)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);
            
            var takenCard = GameState.CurrentBoard.TakeCardFromStockPile(CurrentPlayer);

            if (GameState.CurrentBoard.StockPile.IsEmpty()) 
                MoveDiscardPileToStockPile();

            AddNextTurn(NextPlayer, GetCurrentTurn().SelectedSuit);
            
            return new ActionResultWithCard(true, ActionResultMessageKey.Success, takenCard);
        }

        private Player CurrentPlayer
        {
            get
            {
                if (GetCurrentTurn() == null)
                    return players[GameState.StartingPlayer.Value];
                
                return players[GetCurrentTurn().PlayerToPlay];
            }
        }

        private Player NextPlayer => CurrentPlayer.Number == 1 ? players[2] : players[1];

        private void MoveDiscardPileToStockPile()
        {
            ShuffleDiscardPile();
            GameState.CurrentBoard.MoveDiscardPileToStockPile();
        }

        private void ShuffleDiscardPile()
        {
            var restOfCards = GameState.CurrentBoard.DiscardPile.TakeRestOfCards();
            var turnedUpCard = GameState.CurrentBoard.DiscardPile.CardToMatch;
            var shuffled = new CardCollection(shuffler.Shuffle(restOfCards.Cards));
            shuffled.AddAtStart(turnedUpCard);
            GameState.CurrentBoard.DiscardPile = new DiscardPile(shuffled.Cards);
            GameState.CurrentBoard.DiscardPile.TurnUpTopCard();
        }

        private bool IsValidPlay(Card playedCard)
        {
            var selectedSuit = GetCurrentTurn().SelectedSuit;
            return rules.IsValidPlay(playedCard, GameState.CurrentBoard.DiscardPile.CardToMatch, GetCurrentTurn().TurnNumber,
                selectedSuit);
        }

        private bool IsWinner()
        {
            return CurrentPlayer.Hand.IsEmpty();
        }

        private int? GetWinner()
        {
            int? winner = null;

            if (CurrentPlayer.Hand.IsEmpty())
                winner = CurrentPlayer.Number;

            return winner;
        }

        private Action GetNextAction(IEnumerable<Card> validPlays)
        {
            return !validPlays.Any() ? Action.Take : Action.Play;
        }

        private void AddFirstTurn(Player nextPlayer)
        {
            var winner = GetWinner();
            var validPlays = GetValidPlays(nextPlayer.Hand, GameState.CurrentBoard.DiscardPile.CardToMatch, 1, null).ToArray();

            turns.Add(
                new Turn(1,
                    nextPlayer.Number,
                    GameState.CurrentBoard.StockPile,
                    GameState.CurrentBoard.DiscardPile,
                    players[1].Hand,
                    players[2].Hand,
                    validPlays,
                    IsWinner(),
                    winner,
                    GetNextAction(validPlays), null));
        }

        private void AddNextTurn(Player nextPlayer, Suit? selectedSuit = null)
        {
            var currentTurn = GetCurrentTurn();
            var nextTurnNumber = currentTurn.TurnNumber + 1;
            var validPlays = GetValidPlays(nextPlayer.Hand, GameState.CurrentBoard.DiscardPile.CardToMatch, currentTurn.TurnNumber, selectedSuit)
                .ToArray();

            turns.Add(
                new Turn(nextTurnNumber,
                    nextPlayer.Number,
                    GameState.CurrentBoard.StockPile,
                    GameState.CurrentBoard.DiscardPile,
                    players[1].Hand,
                    players[2].Hand,
                    validPlays,
                    false,
                    null,
                    GetNextAction(validPlays),
                    selectedSuit));
        }

        private void AddCrazyEightTurn()
        {
            var currentTurn = GetCurrentTurn();
            turns.Remove(turns.Last());

            turns.Add(
                new Turn(currentTurn.TurnNumber,
                    currentTurn.PlayerToPlay,
                    GameState.CurrentBoard.StockPile,
                    GameState.CurrentBoard.DiscardPile,
                    players[1].Hand,
                    players[2].Hand,
                    new Card[0],
                    false,
                    null,
                    Action.SelectSuit, null));
        }

        private void AddWinningTurn()
        {
            var currentTurn = GetCurrentTurn();
            turns.Remove(turns.Last());
            turns.Add(
                new Turn(currentTurn.TurnNumber,
                    CurrentPlayer.Number,
                    GameState.CurrentBoard.StockPile,
                    GameState.CurrentBoard.DiscardPile,
                    players[1].Hand,
                    players[2].Hand,
                    new Card[0],
                    true,
                    CurrentPlayer.Number,
                    Action.Won, null));
        }

        private IEnumerable<Card> GetValidPlays(CardCollection hand, Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, hand, turnNumber, selectedSuit);
        }
    }
}
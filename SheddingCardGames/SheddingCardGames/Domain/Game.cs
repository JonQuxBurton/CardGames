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
        private readonly List<Turn> turns;
        private Board board;
        private Player currentPlayer;

        public Game(IRules rules, IShuffler shuffler, IEnumerable<Player> withPlayers)
        {
            this.rules = rules;
            this.shuffler = shuffler;

            foreach (var player in withPlayers)
                players.Add(player.Number, player);

            turns = new List<Turn>();
            GameState = new GameState(GamePhase.New);
        }

        public GameState GameState { get; private set; }
        public IEnumerable<Turn> Turns => turns;

        public IEnumerable<CardMoveEvent> CardMoves => board.CardMoves;

        public Turn GetCurrentTurn()
        {
            return !turns.Any() ? null : turns.Last();
        }

        public void Setup(Board withBoard, int startingPlayer)
        {
            currentPlayer = players[startingPlayer];
            this.board = withBoard;
            
            AddFirstTurn();

            var winner = GetWinner();
            if (winner != null)
                AddWinningTurn();
        }

        public void Deal()
        {
            GameState = new GameState(GamePhase.InGame);
        }
        
        public ActionResult Play(int playerNumber, Card playedCard)
        {
            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);
            
            if (currentPlayer.Number != playerNumber)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (!currentPlayer.Hand.Contains(playedCard))
                return new ActionResult(false, ActionResultMessageKey.CardIsNotInPlayersHand);

            if (!IsValidPlay(playedCard))
                return new ActionResult(false, ActionResultMessageKey.InvalidPlay);

            board.MoveCardToDiscardPile(currentPlayer, playedCard);

            if (IsWinner())
            {
                AddWinningTurn();
            }
            else if (playedCard.Rank == 8)
            {
                AddCrazyEightTurn();
            }
            else
            {
                NextPlayer();
                AddNextTurn();
            }

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public ActionResult SelectSuit(int playerNumber, Suit selectedSuit)
        {
            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);

            if (currentPlayer.Number != playerNumber)
                return new ActionResult(false, ActionResultMessageKey.NotPlayersTurn);
            
            NextPlayer();
            AddNextTurn(selectedSuit);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public ActionResultWithCard Take(int playerNumber)
        {
            if (GetCurrentTurn().NextAction != Action.Take)
                return new ActionResultWithCard(false, ActionResultMessageKey.InvalidTake);

            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);

            if (currentPlayer.Number != playerNumber)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);
            
            var takenCard = board.TakeCardFromStockPile(currentPlayer);

            if (board.StockPile.IsEmpty()) 
                MoveDiscardPileToStockPile();

            NextPlayer();
            AddNextTurn(GetCurrentTurn().SelectedSuit);
            
            return new ActionResultWithCard(true, ActionResultMessageKey.Success, takenCard);
        }

        private void MoveDiscardPileToStockPile()
        {
            ShuffleDiscardPile();
            board.MoveDiscardPileToStockPile();
        }

        private void ShuffleDiscardPile()
        {
            var restOfCards = board.DiscardPile.TakeRestOfCards();
            var turnedUpCard = board.DiscardPile.CardToMatch;
            var shuffled = new CardCollection(shuffler.Shuffle(restOfCards.Cards));
            shuffled.AddAtStart(turnedUpCard);
            board.DiscardPile = new DiscardPile(shuffled.Cards);
            board.DiscardPile.TurnUpTopCard();
        }

        private bool IsValidPlay(Card playedCard)
        {
            var selectedSuit = GetCurrentTurn().SelectedSuit;
            return rules.IsValidPlay(playedCard, board.DiscardPile.CardToMatch, GetCurrentTurn().TurnNumber,
                selectedSuit);
        }

        private bool IsWinner()
        {
            return currentPlayer.Hand.IsEmpty();
        }

        private int? GetWinner()
        {
            int? winner = null;

            if (currentPlayer.Hand.IsEmpty())
                winner = currentPlayer.Number;

            return winner;
        }

        private void NextPlayer()
        {
            currentPlayer = currentPlayer.Number == 1 ? players[2] : players[1];
        }

        private Action GetNextAction(IEnumerable<Card> validPlays)
        {
            return !validPlays.Any() ? Action.Take : Action.Play;
        }

        private void AddFirstTurn()
        {
            var winner = GetWinner();
            var validPlays = GetValidPlays(board.DiscardPile.CardToMatch, 1, null).ToArray();

            turns.Add(
                new Turn(1,
                    currentPlayer.Number,
                    board.StockPile.Cards,
                    board.DiscardPile,
                    players[1].Hand,
                    players[2].Hand,
                    validPlays,
                    IsWinner(),
                    winner,
                    GetNextAction(validPlays), null));
        }

        private void AddNextTurn(Suit? selectedSuit = null)
        {
            var currentTurn = GetCurrentTurn();
            var nextTurnNumber = currentTurn.TurnNumber + 1;
            var validPlays = GetValidPlays(board.DiscardPile.CardToMatch, currentTurn.TurnNumber, selectedSuit)
                .ToArray();

            turns.Add(
                new Turn(nextTurnNumber,
                    currentPlayer.Number,
                    board.StockPile.Cards,
                    board.DiscardPile,
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
                    currentPlayer.Number,
                    board.StockPile.Cards,
                    board.DiscardPile,
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
                    currentPlayer.Number,
                    board.StockPile.Cards,
                    board.DiscardPile,
                    players[1].Hand,
                    players[2].Hand,
                    GetValidPlays(board.DiscardPile.CardToMatch, currentTurn.TurnNumber, null),
                    true,
                    currentPlayer.Number,
                    Action.Won, null));
        }

        private IEnumerable<Card> GetValidPlays(Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, currentPlayer.Hand, turnNumber, selectedSuit);
        }
    }
}
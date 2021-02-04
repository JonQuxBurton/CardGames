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
        private readonly CardCollection deck;

        public Game(IRules rules, IShuffler shuffler, IDealer dealer, IEnumerable<Player> withPlayers, CardCollection deck)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.dealer = dealer;
            this.deck = deck;

            foreach (var player in withPlayers)
                players.Add(player.Number, player);

            gameState = new GameState(GamePhase.New);
        }

        public GameState GameState => gameState;
        private GameState gameState;

        public IEnumerable<CardMoveEvent> CardMoves => GameState.CurrentBoard.CardMoves;

        public void Initialise(GameState initialGameState)
        {
            gameState = initialGameState;
        }
        
        public void ChooseStartingPlayer(int chosenPlayer)
        {
            gameState = new GameState(GamePhase.ReadyToDeal, chosenPlayer);
        }

        public void Deal()
        {
            var shuffled = shuffler.Shuffle(deck);
            var board = dealer.Deal(players.Values, shuffled);
            gameState = gameState
                .WithGamePhase(GamePhase.InGame)
                .WithBoard(board);

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
            if (gameState.CurrentTurn.NextAction != Action.Take)
                return new ActionResultWithCard(false, ActionResultMessageKey.InvalidTake);

            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);

            if (CurrentPlayer.Number != playerNumber)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);
            
            var takenCard = GameState.CurrentBoard.TakeCardFromStockPile(CurrentPlayer);

            if (GameState.CurrentBoard.StockPile.IsEmpty()) 
                MoveDiscardPileToStockPile();

            AddNextTurn(NextPlayer, gameState.CurrentTurn.SelectedSuit);
            
            return new ActionResultWithCard(true, ActionResultMessageKey.Success, takenCard);
        }

        private Player CurrentPlayer
        {
            get
            {
                if (gameState.CurrentTurn == null)
                    return players[GameState.StartingPlayer.Value];
                
                return players[gameState.CurrentTurn.PlayerToPlay];
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
            var shuffled = shuffler.Shuffle(restOfCards);
            shuffled.AddAtStart(turnedUpCard);
            GameState.CurrentBoard.DiscardPile = new DiscardPile(shuffled.Cards);
            GameState.CurrentBoard.DiscardPile.TurnUpTopCard();
        }

        private bool IsValidPlay(Card playedCard)
        {
            var selectedSuit = gameState.CurrentTurn.SelectedSuit;
            return rules.IsValidPlay(playedCard, GameState.CurrentBoard.DiscardPile.CardToMatch, gameState.CurrentTurn.TurnNumber,
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
            
            var newTurn = new Turn(1,
                nextPlayer.Number,
                validPlays,
                IsWinner(),
                winner,
                GetNextAction(validPlays), null);
            gameState = gameState.WithCurrentTurn(newTurn);
        }

        private void AddNextTurn(Player nextPlayer, Suit? selectedSuit = null)
        {
            var currentTurn = gameState.CurrentTurn;
            var nextTurnNumber = currentTurn.TurnNumber + 1;
            var validPlays = GetValidPlays(nextPlayer.Hand, GameState.CurrentBoard.DiscardPile.CardToMatch, currentTurn.TurnNumber, selectedSuit)
                .ToArray();

            var newTurn = 
                new Turn(nextTurnNumber,
                    nextPlayer.Number,
                    validPlays,
                    false,
                    null,
                    GetNextAction(validPlays),
                    selectedSuit);
            gameState = gameState.WithCurrentTurn(newTurn);
        }

        private void AddCrazyEightTurn()
        {
            var currentTurn = gameState.CurrentTurn;

            var newTurn = 
                new Turn(currentTurn.TurnNumber,
                    currentTurn.PlayerToPlay,
                    new Card[0],
                    false,
                    null,
                    Action.SelectSuit, null);
            gameState = gameState.WithCurrentTurn(newTurn);
        }

        private void AddWinningTurn()
        {
            var currentTurn = gameState.CurrentTurn;

            var newTurn = 
                new Turn(currentTurn.TurnNumber,
                    CurrentPlayer.Number,
                    new Card[0],
                    true,
                    CurrentPlayer.Number,
                    Action.Won, null);
            gameState = gameState.WithCurrentTurn(newTurn);
        }

        private IEnumerable<Card> GetValidPlays(CardCollection hand, Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, hand, turnNumber, selectedSuit);
        }
    }
}
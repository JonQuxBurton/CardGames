using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class Game
    {
        private readonly Dictionary<string, Card> cards = new Dictionary<string, Card>();
        private readonly IDealer dealer;
        private readonly CardCollection deck;
        private readonly Dictionary<int, Player> players = new Dictionary<int, Player>();
        private readonly IRules rules;
        private readonly IShuffler shuffler;

        public Game(IRules rules, IShuffler shuffler, IDealer dealer, CardCollection deck,
            Player[] withPlayers)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.dealer = dealer;
            this.deck = deck;

            foreach (var card in deck.Cards)
                if (!cards.ContainsKey($"{card.Rank}|{card.Suit}"))
                    cards.Add($"{card.Rank}|{card.Suit}", card);

            foreach (var player in withPlayers)
                players.Add(player.Number, player);

            GameState = new GameState(GamePhase.New);
        }

        public IEnumerable<DomainEvent> Events => events;
        private readonly List<DomainEvent> events = new List<DomainEvent>();

        public GameState GameState { get; private set; }
        public IEnumerable<CardMoveEvent> CardMoves => GameState.CurrentBoard.CardMoves;


        public Player CurrentPlayer
        {
            get
            {
                if (GameState.CurrentTurn == null)
                    return players[GameState.StartingPlayer.Value];

                return players[GameState.CurrentTurn.PlayerToPlay.Number];
            }
        }

        private Player NextPlayer
        {
            get
            {
                var nextPlayerNumber = CurrentPlayer.Number + 1;

                if (nextPlayerNumber > players.Count)
                    nextPlayerNumber = 1;

                return players[nextPlayerNumber];
            }
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
            events.Add(new Initialised(1));
        }

        public void ChooseStartingPlayer(int chosenPlayer)
        {
            GameState = new GameState(GamePhase.ReadyToDeal, chosenPlayer);
            events.Add(new StartingPlayerChosen(1, chosenPlayer));
        }

        private int GetNextEventNumber()
        {
            return events.Select(x => x.Number).Max() + 1;
        }

        public void Deal()
        {
            var shuffled = shuffler.Shuffle(deck);
            var board = dealer.Deal(players.Values, shuffled);
            GameState = GameState
                .WithGamePhase(GamePhase.InGame)
                .WithBoard(board);
            events.Add(new DealCompleted(GetNextEventNumber()));

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

            events.Add(new Played(GetNextEventNumber(), playerNumber, playedCard));

            if (IsWinner())
            {
                events.Add(new RoundWon(GetNextEventNumber(), playerNumber));
                AddWinningTurn();
            }
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

            events.Add(new SuitSelected(GetNextEventNumber(), playerNumber, selectedSuit));

            AddNextTurn(NextPlayer, selectedSuit);

            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public ActionResultWithCard Take(int playerNumber)
        {
            if (GameState.CurrentTurn.NextAction != Action.Take)
                return new ActionResultWithCard(false, ActionResultMessageKey.InvalidTake);

            if (playerNumber == 0 || playerNumber > players.Count)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);

            if (CurrentPlayer.Number != playerNumber)
                return new ActionResultWithCard(false, ActionResultMessageKey.NotPlayersTurn);

            var takenCard = GameState.CurrentBoard.TakeCardFromStockPile(CurrentPlayer);

            if (GameState.CurrentBoard.StockPile.IsEmpty())
                MoveDiscardPileToStockPile();

            events.Add(new Taken(GetNextEventNumber(), playerNumber, takenCard));

            AddNextTurn(NextPlayer, GameState.CurrentTurn.SelectedSuit);

            return new ActionResultWithCard(true, ActionResultMessageKey.Success, takenCard);
        }

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
            var selectedSuit = GameState.CurrentTurn.SelectedSuit;
            return rules.IsValidPlay(playedCard, GameState.CurrentBoard.DiscardPile.CardToMatch,
                GameState.CurrentTurn.TurnNumber,
                selectedSuit);
        }

        private bool IsWinner()
        {
            return CurrentPlayer.Hand.IsEmpty();
        }

        private Player GetWinner()
        {
            Player winner = null;

            if (CurrentPlayer.Hand.IsEmpty())
                winner = CurrentPlayer;

            return winner;
        }

        private Action GetNextAction(IEnumerable<Card> validPlays)
        {
            return !validPlays.Any() ? Action.Take : Action.Play;
        }

        private void AddFirstTurn(Player nextPlayer)
        {
            var winner = GetWinner();
            var validPlays = GetValidPlays(nextPlayer.Hand, GameState.CurrentBoard.DiscardPile.CardToMatch, 1, null)
                .ToArray();

            var newTurn = new CurrentTurn(1,
                nextPlayer,
                validPlays,
                IsWinner(),
                winner,
                GetNextAction(validPlays), null);
            GameState = GameState.WithCurrentTurn(newTurn);
        }

        private void AddNextTurn(Player nextPlayer, Suit? selectedSuit = null)
        {
            var currentTurn = GameState.CurrentTurn;
            var nextTurnNumber = currentTurn.TurnNumber + 1;
            var validPlays = GetValidPlays(nextPlayer.Hand, GameState.CurrentBoard.DiscardPile.CardToMatch,
                    currentTurn.TurnNumber, selectedSuit)
                .ToArray();

            var newTurn =
                new CurrentTurn(nextTurnNumber,
                    nextPlayer,
                    validPlays,
                    false,
                    null,
                    GetNextAction(validPlays),
                    selectedSuit);
            GameState = GameState.WithCurrentTurn(newTurn);
        }

        private void AddCrazyEightTurn()
        {
            var currentTurn = GameState.CurrentTurn;

            var newTurn =
                new CurrentTurn(currentTurn.TurnNumber,
                    currentTurn.PlayerToPlay,
                    new Card[0],
                    false,
                    null,
                    Action.SelectSuit, null);
            GameState = GameState.WithCurrentTurn(newTurn);
        }

        private void AddWinningTurn()
        {
            var currentTurn = GameState.CurrentTurn;

            var newTurn =
                new CurrentTurn(currentTurn.TurnNumber,
                    CurrentPlayer,
                    new Card[0],
                    true,
                    CurrentPlayer,
                    Action.Won, null);
            GameState = GameState.WithCurrentTurn(newTurn);
        }

        private IEnumerable<Card> GetValidPlays(CardCollection hand, Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, hand, turnNumber, selectedSuit);
        }
    }
}
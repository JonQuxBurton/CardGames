using System;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Action = SheddingCardGames.Domain.Action;

namespace SheddingCardGame.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            IDeckBuilder deckBuilder;
            deckBuilder = new MinimalDeckBuilder();
            //deckBuilder = new DeckBuilder();
            
            var gameBuilder = new CrazyEightsGameBuilder();
            var game = gameBuilder.Build(deckBuilder.Build());
            game.Deal();
            
            Turn currentTurn = null;

            while (currentTurn == null || !currentTurn.HasWinner)
            {
                currentTurn = game.GameState.CurrentTurn;
                RenderTurn(currentTurn, game.GameState);
                //Console.WriteLine($"Moves: {string.Join(",", game.CardMoves)}");
                if (currentTurn.NextAction == Action.SelectSuit)
                    SelectSuit(game, currentTurn);
                else
                    Play(game, currentTurn);
                currentTurn = game.GameState.CurrentTurn;
            }

            Console.WriteLine($"Player {currentTurn.Winner} has won!");
        }

        private static void SelectSuit(Game game, Turn currentTurn)
        {
            Console.WriteLine($"Select a Suit:");
            var selectedSuitInput = Console.ReadLine();
            selectedSuitInput = selectedSuitInput.Trim().ToUpperInvariant();

            var selectedSuit = new SuitParser().Parse(selectedSuitInput);

            Console.WriteLine($"Player {currentTurn.PlayerToPlay} selects Suit: {selectedSuit}");

            game.SelectSuit(currentTurn.PlayerToPlay, selectedSuit);
        }

        private static (int Rank, Suit Suit) ParseInput(string input)
        {
            var rankString = input.Count() > 2 ? input.Substring(0, 2) : input.Substring(0, 1);
            var suitString = input.Last().ToString().Trim().ToUpperInvariant();
            var rank = int.Parse(rankString);
            var suit = new SuitParser().Parse(suitString);

            return (rank, suit);
        }

        private static void Play(Game game, Turn currentTurn)
        {
            if (!currentTurn.ValidPlays.Any())
            {
                Console.WriteLine($"No valid plays, press any key to Take a card");
                Console.ReadKey();
                var actionResult  = game.Take(currentTurn.PlayerToPlay);
                Console.WriteLine($"Taken: {actionResult.Card}");
                return;
            }
            
            Console.WriteLine($"Enter Player {currentTurn.PlayerToPlay}'s play in format RS (RankSuit):");
            var playInput = Console.ReadLine();

            var parsedInput = ParseInput(playInput);
            var playInputRank = parsedInput.Rank;
            var playSuit = parsedInput.Suit;

            var play = new Card(playInputRank, playSuit);

            Console.WriteLine($"Player {currentTurn.PlayerToPlay} plays: {play}");

            var playResult = game.Play(currentTurn.PlayerToPlay, play);
            Console.WriteLine($"IsValidPlay: {playResult}");
        }

        private static void RenderTurn(Turn turn, GameState gameState)
        {
            var currentBoard = gameState.CurrentBoard;

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Turn {turn.TurnNumber}");
            Console.WriteLine($"PlayerToPlay: {turn.PlayerToPlay}");
            Console.WriteLine($"NextAction: {turn.NextAction}");
            Console.WriteLine($"SelectedSuit: {turn.SelectedSuit}");
            Console.WriteLine($"Stock pile: {currentBoard.StockPile.Cards.Count()} cards");
            Console.WriteLine($"Discard pile: {currentBoard.DiscardPile.CardToMatch} ({currentBoard.DiscardPile.RestOfCards.Cards.Count()} other cards)");
            
            var player1Hand = string.Join(", ", currentBoard.Player1.Hand.Cards.Select(x => x.ToString()));
            Console.WriteLine($"Player 1 hand: {player1Hand} ");

            var player2Hand = string.Join(", ", currentBoard.Player2.Hand.Cards.Select(x => x.ToString()));
            Console.WriteLine($"Player 2 hand: {player2Hand} ");

            Console.WriteLine($"Player {turn.PlayerToPlay} potential plays:");
            if (!turn.ValidPlays.Any())
            {
                Console.WriteLine("None");
            }
            else
            {
                foreach (var validPlay in turn.ValidPlays)
                {
                    Console.WriteLine($"{validPlay}");
                }
            }

            Console.WriteLine("--------------------------------------------------");
        }
    }
}

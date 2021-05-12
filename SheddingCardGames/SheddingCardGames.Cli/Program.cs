using System;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.Suit;
using Action = SheddingCardGames.Domain.Action;

namespace SheddingCardGame.Cli
{
    class Program
    {
        private static Game SetupGame()
        {
            var numberOfPlayers = 2;
            IDeckBuilder deckBuilder = new DeckBuilder();
            ICrazyEightsGameBuilder gameBuilder = new CrazyEightsGameBuilder();

            return gameBuilder.Build(deckBuilder.Build(), numberOfPlayers);
        }
        
        private static Game SetupMinimalGame()
        {
            var numberOfPlayers = 2;
            IDeckBuilder deckBuilder = new MinimalDeckBuilder(numberOfPlayers);
            ICrazyEightsGameBuilder gameBuilder = new CrazyEightsGameBuilder();

            return gameBuilder.Build(deckBuilder.Build(), numberOfPlayers);
        }
        
        private static Game SetupTestGame()
        {
            var numberOfPlayers = 2;
            IDeckBuilder deckBuilder = new SpecificDeckBuilder(Card(1, Hearts), new CardCollection(Cards(Card(1, Spades))),
                new CardCollection(Cards(Card(8, Clubs), Card(2, Clubs))),
                new CardCollection(Cards(Card(3, Clubs), Card(5, Clubs))));
            ICrazyEightsGameBuilder gameBuilder = new TestCrazyEightsGameBuilder(2, 1);

            return gameBuilder.Build(deckBuilder.Build(), numberOfPlayers);
        }

        static void Main(string[] args)
        {
            var game = SetupTestGame();
            game.Deal();

            PreviousTurnResult previousTurnResult = null;

            while (previousTurnResult == null || !previousTurnResult.HasWinner)
            {
                var currentTurn = game.GameState.CurrentTurn;
                previousTurnResult = game.GameState.PreviousTurnResult;
                RenderTurn(currentTurn, game.GameState);
                if (currentTurn.NextAction == Action.SelectSuit)
                    SelectSuit(game, currentTurn);
                else
                    Play(game, currentTurn);
            }

            Console.WriteLine($"{previousTurnResult.Winner.Name} has won!");
        }

        private static void SelectSuit(Game game, CurrentTurn currentTurn)
        {
            Console.WriteLine($"Select a Suit:");
            var selectedSuitInput = Console.ReadLine();
            selectedSuitInput = selectedSuitInput.Trim().ToUpperInvariant();

            var selectedSuit = new SuitParser().Parse(selectedSuitInput);

            Console.WriteLine($"{currentTurn.PlayerToPlay.Name} selects Suit: {selectedSuit}");

            game.SelectSuit(new SelectSuitContext(currentTurn.PlayerToPlay, selectedSuit));
        }

        private static (int Rank, Suit Suit) ParseInput(string input)
        {
            var rankString = input.Count() > 2 ? input.Substring(0, 2) : input.Substring(0, 1);
            var suitString = input.Last().ToString().Trim().ToUpperInvariant();
            var rank = int.Parse(rankString);
            var suit = new SuitParser().Parse(suitString);

            return (rank, suit);
        }

        private static void Play(Game game, CurrentTurn currentTurn)
        {
            if (currentTurn.NextAction == Action.Take)
            {
                Console.WriteLine($"No valid plays, press any key to Take a card");
                Console.ReadKey();
                game.Take(new TakeContext(currentTurn.PlayerToPlay));
                Console.WriteLine($"Taken: {game.GameState.PreviousTurnResult.TakenCard}");
                return;
            }
            
            Console.WriteLine($"Enter {currentTurn.PlayerToPlay.Name}'s play in format RS (RankSuit):");
            var playInput = Console.ReadLine();

            var parsedInput = ParseInput(playInput);
            var playInputRank = parsedInput.Rank;
            var playSuit = parsedInput.Suit;

            var play = new Card(playInputRank, playSuit);

            Console.WriteLine($"{currentTurn.PlayerToPlay.Name} plays: {play}");

            var playResult = game.Play(new PlayContext(currentTurn.PlayerToPlay, Cards(play)));
            Console.WriteLine($"IsValidPlay: {playResult}");
        }

        private static void RenderTurn(CurrentTurn turn, GameState gameState)
        {
            var currentTable = gameState.CurrentTable;

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Turn {turn.TurnNumber}");
            Console.WriteLine($"PlayerToPlay: {turn.PlayerToPlay.Name}");
            Console.WriteLine($"NextAction: {turn.NextAction}");
            Console.WriteLine($"SelectedSuit: {gameState.PreviousTurnResult?.SelectedSuit}");
            Console.WriteLine($"Stock pile: {currentTable.StockPile.Cards.Count()} cards");
            Console.WriteLine($"Discard pile: {currentTable.DiscardPile.CardToMatch} ({currentTable.DiscardPile.RestOfCards.Cards.Count()} other cards)");
            
            foreach (var player in currentTable.Players)
            {
                var playerHand = string.Join(", ", player.Hand.Cards.Select(x => x.ToString()));
                Console.WriteLine($"Player {player.Number} ({player.Name}) hand: {playerHand} ");
            }
            
            Console.WriteLine("--------------------------------------------------");
        }
    }
}

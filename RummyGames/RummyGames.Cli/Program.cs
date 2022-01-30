using System;
using CardGamesDomain;

namespace RummyGames.Cli
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dataStore = new DataStore();
            var lobby = new Lobby(dataStore, new StartingPlayerChooser());
            var player1 = new Player(Guid.NewGuid(), "Alice");

            var game = lobby.CreateGame(player1);

            var player2 = new Player(Guid.NewGuid(), "Bob");

            game = lobby.JoinGame(game, player2);
            game = lobby.SetupGame(game);

            var inGameState = dataStore.GetInGameState(game.Id);

            var controller = new InGameController(new RandomShuffler());
            inGameState = controller.ShuffleDeck(inGameState);

            Console.WriteLine("Rummy");
            Console.WriteLine($"Host: {game.Host.Name}, Guest: {game.Guest.Name}");
            Console.WriteLine($"Starting Player: {inGameState.StartingPlayer.Name}");

            var deckString = string.Join(',', inGameState.Table.Deck.Cards);
            Console.WriteLine($"Deck: {deckString}");

            Console.WriteLine("Dealing...");
            inGameState = controller.Deal(inGameState);

            foreach (var player in inGameState.Table.Players)
                Console.WriteLine($"{player.Name}: {player.Hand}");

            Console.WriteLine("Press 1 to Take From StockPile");
            
            var selection = 0;
            var input = Console.ReadKey();

            if (char.IsDigit(input.KeyChar))
            {
                selection = int.Parse(input.KeyChar.ToString());
            }

            if (selection == 1)
            {
                Console.WriteLine("\nTake From StockPile");
                var result = controller.TakeFromStockPile(inGameState, inGameState.CurrentTurn.CurrentPlayer);
                Console.WriteLine($"Taken Card: {result.NewInGameState.CurrentTurn.TakenCard}");
            }

        }
    }
}
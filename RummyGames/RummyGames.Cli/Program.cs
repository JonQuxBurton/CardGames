using System;

namespace RummyGames.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataStore = new DataStore();
            var lobby = new Lobby(dataStore);
            var player1 = new Player(Guid.NewGuid(), "Alice");

            var game = lobby.CreateGame(player1);

            var player2 = new Player(Guid.NewGuid(), "Bob");

            game = lobby.JoinGame(game, player2);


            Console.WriteLine("Rummy");
            Console.WriteLine($"Host: {game.Host.Name}, Guest: {game.Guest.Name}");
        }
    }
}

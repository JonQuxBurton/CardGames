﻿using System;
using CardGamesDomain;

namespace RummyGames.Cli
{
    class Program
    {
        static void Main(string[] args)
        {
            var dataStore = new DataStore();
            var lobby = new Lobby(dataStore, new StartingPlayerChooser());
            var player1 = new Player(Guid.NewGuid(), "Alice");

            var game = lobby.CreateGame(player1);

            var player2 = new Player(Guid.NewGuid(), "Bob");

            game = lobby.JoinGame(game, player2);
            game = lobby.SetupGame(game);

            var inGameState = dataStore.GetInGameState(game.Id);

            var inGameController = new InGameController(new RandomShuffler());
            inGameState = inGameController.ShuffleDeck(inGameState);

            Console.WriteLine("Rummy");
            Console.WriteLine($"Host: {game.Host.Name}, Guest: {game.Guest.Name}");
            Console.WriteLine($"Starting Player: {inGameState.StartingPlayer.Name}");

            var deckString = string.Join(',', inGameState.Table.Deck.Cards);
            Console.WriteLine($"Deck: {deckString}");

        }
    }
}

using System;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace RummyGames.Test
{
    public class LobbySpec
    {
        [Fact]
        public void CreateGame_Should_CreateNewGame()
        {
            var host = new Player(Guid.NewGuid(), "Alice");
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore);

            var actual = sut.CreateGame(host);

            actual.Should().NotBeNull();
            actual.Host.Should().Be(host);
        }

        [Fact]
        public void JoinGame_Should_JoinGame()
        {
            var host = new Player(Guid.NewGuid(), "Alice");
            var guest = new Player(Guid.NewGuid(), "Bob");
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore);
            var game = sut.CreateGame(host);
            
            var actual = sut.JoinGame(game, guest);

            actual.Guest.Should().Be(guest);
        }
        
        [Fact]
        public void JoinGame_Should_CreateInGameState()
        {
            var host = new Player(Guid.NewGuid(), "Alice");
            var guest = new Player(Guid.NewGuid(), "Bob");
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore);
            var game = sut.CreateGame(host);
            
            sut.JoinGame(game, guest);

            var actual = dataStore.GetInGameState(game.Id);

            actual.Should().NotBeNull();
            actual.GameId.Should().Be(game.Id);
            actual.StartingPlayer.Should().Be(host);
        }
    }
}

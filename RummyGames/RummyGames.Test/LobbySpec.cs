using System;
using FluentAssertions;
using Xunit;

namespace RummyGames.Test
{
    public class LobbySpec
    {
        private readonly Player guest;
        private readonly Player host;

        public LobbySpec()
        {
            host = new Player(Guid.NewGuid(), "Alice");
            guest = new Player(Guid.NewGuid(), "Bob");
        }

        public Lobby CreateSut()
        {
            var dataStore = new DataStore();
            return new Lobby(dataStore, new StartingPlayerChooser());
        }

        [Fact]
        public void CreateGame_Should_CreateNewGame()
        {
            var sut = CreateSut();

            var actual = sut.CreateGame(host);

            actual.Should().NotBeNull();
            actual.Host.Should().Be(host);
        }

        [Fact]
        public void JoinGame_Should_JoinGame()
        {
            var sut = CreateSut();
            var game = sut.CreateGame(host);

            var actual = sut.JoinGame(game, guest);

            actual.Guest.Should().Be(guest);
        }

        [Fact]
        public void JoinGame_Should_CreateInGameState()
        {
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore, new StartingPlayerChooser());
            var game = sut.CreateGame(host);

            sut.JoinGame(game, guest);

            var actual = dataStore.GetInGameState(game.Id);

            actual.Should().NotBeNull();
            actual.GameId.Should().Be(game.Id);
            actual.StartingPlayer.Should().Be(host);
        }

        [Fact]
        public void JoinGame_Should_ChooseStartingPlayerAsPlayer1()
        {
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore, new FakeStartingPlayerChooser(host));
            var game = sut.CreateGame(host);

            sut.JoinGame(game, guest);

            var actual = dataStore.GetInGameState(game.Id);

            actual.StartingPlayer.Should().Be(host);
        }

        [Fact]
        public void JoinGame_Should_ChooseStartingPlayerAsPlayer2()
        {
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore, new FakeStartingPlayerChooser(guest));
            var game = sut.CreateGame(host);

            sut.JoinGame(game, guest);

            var actual = dataStore.GetInGameState(game.Id);

            actual.StartingPlayer.Should().Be(guest);
        }
    }
}
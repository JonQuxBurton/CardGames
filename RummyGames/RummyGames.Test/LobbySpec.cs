using System;
using CardGamesDomain;
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
        public void SetupGame_Should_CreateInGameState()
        {
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore, new StartingPlayerChooser());
            var game = sut.CreateGame(host);
            game = sut.JoinGame(game, guest);

            sut.SetupGame(game);

            var actual = dataStore.GetInGameState(game.Id);
            actual.Should().NotBeNull();
            actual.GameId.Should().Be(game.Id);
            actual.StartingPlayer.Should().Be(host);
        }

        [Fact]
        public void SetupGame_Should_ChooseStartingPlayerAsPlayer1()
        {
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore, new FakeStartingPlayerChooser(host));
            var game = sut.CreateGame(host);
            game = sut.JoinGame(game, guest);

            sut.SetupGame(game);

            var actual = dataStore.GetInGameState(game.Id);

            actual.StartingPlayer.Should().Be(host);
            actual.CurrentTurn.Should().BeNull();
        }

        [Fact]
        public void SetupGame_Should_ChooseStartingPlayerAsPlayer2()
        {
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore, new FakeStartingPlayerChooser(guest));
            var game = sut.CreateGame(host);
            game = sut.JoinGame(game, guest);

            sut.SetupGame(game);

            var actual = dataStore.GetInGameState(game.Id);

            actual.StartingPlayer.Should().Be(guest);
            actual.CurrentTurn.Should().BeNull();
        }

        [Fact]
        public void SetupGame_Should_CreateTable()
        {
            var deckBuilder = new DeckBuilder();
            var dataStore = new DataStore();
            var sut = new Lobby(dataStore, new StartingPlayerChooser());
            var game = sut.CreateGame(host);
            game = sut.JoinGame(game, guest);

            sut.SetupGame(game);

            var actual = dataStore.GetInGameState(game.Id);

            actual.Table.Should().NotBeNull();
            actual.Table.Players.Should().Equal(host, guest);
            actual.Table.Deck.Should().NotBeNull();
            actual.Table.Deck.Cards.Should().Equal(deckBuilder.Build().Cards);
        }
    }
}
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Xunit;

namespace SheddingCardGames.Tests.EndToEnd
{
    public class PlayAGame
    {
        private Game sut;

        [Fact]
        public void WherePlayer1Wins()
        {
            var player1Hand = new CardCollection(
                new Card(1, Suit.Diamonds),
                new Card(2, Suit.Hearts),
                new Card(3, Suit.Diamonds),
                new Card(4, Suit.Hearts),
                new Card(5, Suit.Diamonds),
                new Card(6, Suit.Hearts),
                new Card(7, Suit.Diamonds)
            );
            var player2Hand = new CardCollection(
                new Card(1, Suit.Spades),
                new Card(2, Suit.Diamonds),
                new Card(3, Suit.Hearts),
                new Card(4, Suit.Diamonds),
                new Card(5, Suit.Hearts),
                new Card(6, Suit.Diamonds),
                new Card(7, Suit.Hearts)
            );
            var discardCard = new Card(1, Suit.Clubs);
            var stockPile = new CardCollection(
                new Card(8, Suit.Clubs),
                new Card(8, Suit.Spades)
            );
            var sampleData = new SampleData();
            var player1 = sampleData.Player1;
            var player2 = sampleData.Player2;
            
            sut = CreateSut(player1, player1Hand, player2, player2Hand, discardCard, stockPile);
            
            sut.ChooseStartingPlayer(new ChooseStartingPlayerContext(player1));
            sut.Deal();

            sut.GameState.CurrentTurn.TurnNumber.Should().Be(1);
            sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            sut.GameState.CurrentTable.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(stockPile.Cards);

            var result = sut.Play(new PlayContext(player1, Card(1, Suit.Diamonds)));
            VerifyPlayerPlay(1, result, 2, Card(1, Suit.Diamonds), new[]
            {
                Card(2, Suit.Hearts),
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(1, Suit.Spades)));
            VerifyPlayerPlay(2, result, 3, Card(1, Suit.Spades), new[]
            {
                Card(2, Suit.Diamonds),
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            var takeResult = sut.Take(new TakeContext(player1));
            VerifyPlayerTake(1, takeResult, 4, Card(8, Suit.Clubs), new[]
            {
                Card(2, Suit.Hearts),
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds),
                Card(8, Suit.Clubs)
            });

            takeResult = sut.Take(new TakeContext(player2));
            VerifyPlayerTake(2, takeResult, 5, Card(8, Suit.Spades), new[]
            {
                Card(2, Suit.Diamonds),
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts),
                Card(8, Suit.Spades)
            });

            result = sut.Play(new PlayContext(player1, Card(8, Suit.Clubs)));
            VerifyPlayerPlay(1, result, 5, Card(8, Suit.Clubs), new[]
            {
                Card(2, Suit.Hearts),
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.SelectSuit(new SelectSuitContext(player1, Suit.Spades));

            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(6);
            sut.GameState.PreviousTurnResult.SelectedSuit.Should().Be(Suit.Spades);

            result = sut.Play(new PlayContext(player2, Card(8, Suit.Spades)));
            VerifyPlayerPlay(2, result, 6, Card(8, Suit.Spades), new[]
            {
                Card(2, Suit.Diamonds),
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.SelectSuit(new SelectSuitContext(player2, Suit.Hearts));

            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(7);
            sut.GameState.PreviousTurnResult.SelectedSuit.Should().Be(Suit.Hearts);

            result = sut.Play(new PlayContext(player1, Card(2, Suit.Hearts)));
            VerifyPlayerPlay(1, result, 8, Card(2, Suit.Hearts), new[]
            {
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(2, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 9, Card(2, Suit.Diamonds), new[]
            {
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(3, Suit.Diamonds)));
            VerifyPlayerPlay(1, result, 10, Card(3, Suit.Diamonds), new[]
            {
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(3, Suit.Hearts)));
            VerifyPlayerPlay(2, result, 11, Card(3, Suit.Hearts), new[]
            {
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(4, Suit.Hearts)));
            VerifyPlayerPlay(1, result, 12, Card(4, Suit.Hearts), new[]
            {
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(4, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 13, Card(4, Suit.Diamonds), new[]
            {
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(5, Suit.Diamonds)));
            VerifyPlayerPlay(1, result, 14, Card(5, Suit.Diamonds), new[]
            {
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(5, Suit.Hearts)));
            VerifyPlayerPlay(2, result, 15, Card(5, Suit.Hearts), new[]
            {
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });


            result = sut.Play(new PlayContext(player1, Card(6, Suit.Hearts)));
            VerifyPlayerPlay(1, result, 16, Card(6, Suit.Hearts), new[]
            {
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(6, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 17, Card(6, Suit.Diamonds), new[]
            {
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(7, Suit.Diamonds)));
            VerifyPlayerWon(1, result, 17, Card(7, Suit.Diamonds));
        }

        [Fact]
        public void WherePlayer2Wins()
        {
            var player1Hand = new CardCollection(
                new Card(1, Suit.Diamonds),
                new Card(2, Suit.Hearts),
                new Card(3, Suit.Diamonds),
                new Card(4, Suit.Hearts),
                new Card(5, Suit.Diamonds),
                new Card(6, Suit.Hearts),
                new Card(7, Suit.Diamonds)
            );
            var player2Hand = new CardCollection(
                new Card(1, Suit.Spades),
                new Card(2, Suit.Diamonds),
                new Card(3, Suit.Hearts),
                new Card(4, Suit.Diamonds),
                new Card(5, Suit.Hearts),
                new Card(6, Suit.Diamonds),
                new Card(7, Suit.Hearts)
            );
            var discardCard = new Card(1, Suit.Clubs);
            var stockPile = new CardCollection(
                new Card(8, Suit.Clubs),
                new Card(8, Suit.Spades)
            );
            var sampleData = new SampleData();
            var player1 = sampleData.Player1;
            var player2 = sampleData.Player2;

            sut = CreateSut(player1, player1Hand, player2, player2Hand, discardCard, stockPile);
            
            sut.ChooseStartingPlayer(new ChooseStartingPlayerContext(player2));
            sut.Deal();

            sut.GameState.CurrentTurn.TurnNumber.Should().Be(1);
            sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            sut.GameState.CurrentTable.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(stockPile.Cards);

            var result = sut.Play(new PlayContext(player2, Card(1, Suit.Spades)));
            VerifyPlayerPlay(2, result, 2, Card(1, Suit.Spades), new[]
            {
                Card(2, Suit.Diamonds),
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(1, Suit.Diamonds)));
            VerifyPlayerPlay(1, result, 3, Card(1, Suit.Diamonds), new[]
            {
                    Card(2, Suit.Hearts),
                    Card(3, Suit.Diamonds),
                    Card(4, Suit.Hearts),
                    Card(5, Suit.Diamonds),
                    Card(6, Suit.Hearts),
                    Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(2, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 4, Card(2, Suit.Diamonds), new[]
            {
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(2, Suit.Hearts)));
            VerifyPlayerPlay(1, result, 5, Card(2, Suit.Hearts), new[]
            {
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(3, Suit.Hearts)));
            VerifyPlayerPlay(2, result, 6, Card(3, Suit.Hearts), new[]
            {
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(3, Suit.Diamonds)));
            VerifyPlayerPlay(1, result, 7, Card(3, Suit.Diamonds), new[]
            {
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });
            
            result = sut.Play(new PlayContext(player2, Card(4, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 8, Card(4, Suit.Diamonds), new[]
            {
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(4, Suit.Hearts)));
            VerifyPlayerPlay(1, result, 9, Card(4, Suit.Hearts), new[]
            {
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(5, Suit.Hearts)));
            VerifyPlayerPlay(2, result, 10, Card(5, Suit.Hearts), new[]
            {
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(5, Suit.Diamonds)));
            VerifyPlayerPlay(1, result,11, Card(5, Suit.Diamonds), new[]
            {
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(6, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 12, Card(6, Suit.Diamonds), new[]
            {
                Card(7, Suit.Hearts)
            });

            result = sut.Play(new PlayContext(player1, Card(6, Suit.Hearts)));
            VerifyPlayerPlay(1, result, 13, Card(6, Suit.Hearts), new[]
            {
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(new PlayContext(player2, Card(7, Suit.Hearts)));
            VerifyPlayerWon(2, result, 13, Card(7, Suit.Hearts));
        }

        [Fact]
        public void WherePlayer3Wins()
        {
            var player1Hand = new CardCollection(
                new Card(1, Suit.Clubs),
                new Card(3, Suit.Diamonds),
                new Card(5, Suit.Clubs),
                new Card(7, Suit.Diamonds),
                new Card(9, Suit.Clubs)
            );
            var player2Hand = new CardCollection(
                new Card(3, Suit.Clubs),
                new Card(5, Suit.Diamonds),
                new Card(7, Suit.Clubs),
                new Card(9, Suit.Diamonds),
                new Card(11, Suit.Clubs)
            );
            var player3Hand = new CardCollection(
                new Card(1, Suit.Spades),
                new Card(3, Suit.Spades),
                new Card(5, Suit.Spades),
                new Card(7, Suit.Spades),
                new Card(9, Suit.Spades)
            );
            var discardCard = new Card(1, Suit.Hearts);
            var stockPile = new CardCollection(
                new Card(2, Suit.Hearts),
                new Card(3, Suit.Hearts),
                new Card(4, Suit.Hearts)
            );

            var player1 = new Player(1, "Alice");
            var player2 = new Player(2, "Bob");
            var player3 = new Player(3, "Carol");

            IDeckBuilder deckBuilder = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand);
            var deck = deckBuilder.Build();

            var players = new[] { player1, player2, player3 };
            var rules = new Rules(5);
            var shuffler = new DummyShuffler();

            sut = new Game(rules, shuffler, deck, players);
            
            sut.ChooseStartingPlayer(new ChooseStartingPlayerContext(player3));
            sut.Deal();

            sut.GameState.CurrentTurn.TurnNumber.Should().Be(1);
            sut.GameState.CurrentTable.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            sut.GameState.CurrentTable.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            sut.GameState.CurrentTable.Players[2].Hand.Cards.Should().Equal(player3Hand.Cards);
            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(stockPile.Cards);

            var result = sut.Play(new PlayContext(player3, Card(1, Suit.Spades)));
            VerifyPlayerPlay(3, result, 2, Card(1, Suit.Spades), new[]
            {
                new Card(3, Suit.Spades),
                new Card(5, Suit.Spades),
                new Card(7, Suit.Spades),
                new Card(9, Suit.Spades)
            });

            result = sut.Play(new PlayContext(player1, Card(1, Suit.Clubs)));
            VerifyPlayerPlay(1, result, 3, Card(1, Suit.Clubs), new[]
            {
                new Card(3, Suit.Diamonds),
                new Card(5, Suit.Clubs),
                new Card(7, Suit.Diamonds),
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player2, Card(3, Suit.Clubs)));
            VerifyPlayerPlay(2, result, 4, Card(3, Suit.Clubs), new[]
            {
                new Card(5, Suit.Diamonds),
                new Card(7, Suit.Clubs),
                new Card(9, Suit.Diamonds),
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player3, Card(3, Suit.Spades)));
            VerifyPlayerPlay(3, result, 5, Card(3, Suit.Spades), new[]
            {
                new Card(5, Suit.Spades),
                new Card(7, Suit.Spades),
                new Card(9, Suit.Spades)
            });

            result = sut.Play(new PlayContext(player1, Card(3, Suit.Diamonds)));
            VerifyPlayerPlay(1, result, 6, Card(3, Suit.Diamonds), new[]
            {
                new Card(5, Suit.Clubs),
                new Card(7, Suit.Diamonds),
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player2, Card(5, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 7, Card(5, Suit.Diamonds), new[]
            {
                new Card(7, Suit.Clubs),
                new Card(9, Suit.Diamonds),
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player3, Card(5, Suit.Spades)));
            VerifyPlayerPlay(3, result, 8, Card(5, Suit.Spades), new[]
            {
                new Card(7, Suit.Spades),
                new Card(9, Suit.Spades)
            });

            result = sut.Play(new PlayContext(player1, Card(5, Suit.Clubs)));
            VerifyPlayerPlay(1, result, 9, Card(5, Suit.Clubs), new[]
            {
                new Card(7, Suit.Diamonds),
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player2, Card(7, Suit.Clubs)));
            VerifyPlayerPlay(2, result, 10, Card(7, Suit.Clubs), new[]
            {
                new Card(9, Suit.Diamonds),
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player3, Card(7, Suit.Spades)));
            VerifyPlayerPlay(3, result, 11, Card(7, Suit.Spades), new[]
            {
                new Card(9, Suit.Spades)
            });

            result = sut.Play(new PlayContext(player1, Card(7, Suit.Diamonds)));
            VerifyPlayerPlay(1, result, 12, Card(7, Suit.Diamonds), new[]
            {
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player2, Card(9, Suit.Diamonds)));
            VerifyPlayerPlay(2, result, 13, Card(9, Suit.Diamonds), new[]
            {
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(new PlayContext(player3, Card(9, Suit.Spades)));
            VerifyPlayerWon(3, result, 13, Card(9, Suit.Spades));
        }

        private Game CreateSut(Player player1, CardCollection player1Hand, Player player2, CardCollection player2Hand, Card discardCard,
            CardCollection stockPile)
        {
            IDeckBuilder deckBuilder = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand);
            var deck = deckBuilder.Build();

            var players = new[] {player1, player2};
            var rules = new Rules(7);
            var shuffler = new DummyShuffler();

            return new Game(rules, shuffler, deck, players);
        }

        private Card Card(int rank, Suit suit)
        {
            return sut.GetCard(rank, suit);
        }

        private void VerifyPlayerTake(int playerNumber, ActionResult takeResult, int turnNumber,
            Card takenCard,
            Card[] hand)
        {
            takeResult.IsSuccess.Should().BeTrue();
            sut.GameState.PreviousTurnResult.TakenCard.Should().Be(takenCard);
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentTable.Players[playerNumber-1].Hand.Cards.Should().Equal(hand);
        }

        private void VerifyPlayerWon(int playerNumber, ActionResult result, int turnNumber, Card discardCard)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentTable.Players[playerNumber-1].Hand.Cards.Should().BeEmpty();
            sut.GameState.PreviousTurnResult.HasWinner.Should().BeTrue();
            sut.GameState.PreviousTurnResult.Winner.Number.Should().Be(playerNumber);
        }

        private void VerifyPlayerPlay(int playerNumber, ActionResult result, int turnNumber, Card discardCard, Card[] hand)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentTable.Players[playerNumber-1].Hand.Cards.Should().Equal(hand);
        }
    }
}
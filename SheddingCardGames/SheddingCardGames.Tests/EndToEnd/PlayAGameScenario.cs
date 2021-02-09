using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Xunit;

namespace SheddingCardGames.Tests.EndToEnd
{
    public class PlayAGameScenario
    {
        private Game sut;

        [Fact]
        public void PlayAGame()
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

            IDeckBuilder deckBuilder = new SpecificDeckBuilder(player1Hand, player2Hand, discardCard, stockPile);
            var deck = deckBuilder.Build();

            var players = new[] {new Player(1), new Player(2)};
            var rules = new Rules(7);
            var shuffler = new DummyShuffler();
            var dealer = new Dealer(rules);

            sut = new Game(rules, shuffler, dealer, players, deck);

            sut.ChooseStartingPlayer(1);
            sut.Deal();

            sut.GameState.CurrentTurn.TurnNumber.Should().Be(1);
            sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().Equal(player1Hand.Cards);
            sut.GameState.CurrentBoard.Player2.Hand.Cards.Should().Equal(player2Hand.Cards);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.StockPile.Cards.Should().Equal(stockPile.Cards);

            var result = sut.Play(1, sut.GetCard(1, Suit.Diamonds));
            VerifyPlayer1Play(result, 2, sut.GetCard(1, Suit.Diamonds), new[]
            {
                sut.GetCard(2, Suit.Hearts),
                sut.GetCard(3, Suit.Diamonds),
                sut.GetCard(4, Suit.Hearts),
                sut.GetCard(5, Suit.Diamonds),
                sut.GetCard(6, Suit.Hearts),
                sut.GetCard(7, Suit.Diamonds)
            });

            result = sut.Play(2, sut.GetCard(1, Suit.Spades));
            VerifyPlayer2Play(result, 3, sut.GetCard(1, Suit.Spades), new[]
            {
                sut.GetCard(2, Suit.Diamonds),
                sut.GetCard(3, Suit.Hearts),
                sut.GetCard(4, Suit.Diamonds),
                sut.GetCard(5, Suit.Hearts),
                sut.GetCard(6, Suit.Diamonds),
                sut.GetCard(7, Suit.Hearts)
            });

            var takeResult = sut.Take(1);
            VerifyPlayer1Take(takeResult, 4, sut.GetCard(8, Suit.Clubs), new[]
            {
                sut.GetCard(2, Suit.Hearts),
                sut.GetCard(3, Suit.Diamonds),
                sut.GetCard(4, Suit.Hearts),
                sut.GetCard(5, Suit.Diamonds),
                sut.GetCard(6, Suit.Hearts),
                sut.GetCard(7, Suit.Diamonds),
                sut.GetCard(8, Suit.Clubs)
            });

            takeResult = sut.Take(2);
            VerifyPlayer2Take(takeResult, 5, sut.GetCard(8, Suit.Spades), new[]
            {
                sut.GetCard(2, Suit.Diamonds),
                sut.GetCard(3, Suit.Hearts),
                sut.GetCard(4, Suit.Diamonds),
                sut.GetCard(5, Suit.Hearts),
                sut.GetCard(6, Suit.Diamonds),
                sut.GetCard(7, Suit.Hearts),
                sut.GetCard(8, Suit.Spades)
            });

            result = sut.Play(1, sut.GetCard(8, Suit.Clubs));
            VerifyPlayer1Play(result, 5, sut.GetCard(8, Suit.Clubs), new[]
            {
                sut.GetCard(2, Suit.Hearts),
                sut.GetCard(3, Suit.Diamonds),
                sut.GetCard(4, Suit.Hearts),
                sut.GetCard(5, Suit.Diamonds),
                sut.GetCard(6, Suit.Hearts),
                sut.GetCard(7, Suit.Diamonds)
            });

            result = sut.SelectSuit(1, Suit.Spades);

            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(6);
            sut.GameState.CurrentTurn.SelectedSuit.Should().Be(Suit.Spades);

            result = sut.Play(2, sut.GetCard(8, Suit.Spades));
            VerifyPlayer2Play(result, 6, sut.GetCard(8, Suit.Spades), new[]
            {
                sut.GetCard(2, Suit.Diamonds),
                sut.GetCard(3, Suit.Hearts),
                sut.GetCard(4, Suit.Diamonds),
                sut.GetCard(5, Suit.Hearts),
                sut.GetCard(6, Suit.Diamonds),
                sut.GetCard(7, Suit.Hearts)
            });

            result = sut.SelectSuit(2, Suit.Hearts);

            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(7);
            sut.GameState.CurrentTurn.SelectedSuit.Should().Be(Suit.Hearts);

            result = sut.Play(1, sut.GetCard(2, Suit.Hearts));
            VerifyPlayer1Play(result, 8, sut.GetCard(2, Suit.Hearts), new[]
            {
                sut.GetCard(3, Suit.Diamonds),
                sut.GetCard(4, Suit.Hearts),
                sut.GetCard(5, Suit.Diamonds),
                sut.GetCard(6, Suit.Hearts),
                sut.GetCard(7, Suit.Diamonds)
            });

            result = sut.Play(2, sut.GetCard(2, Suit.Diamonds));
            VerifyPlayer2Play(result, 9, sut.GetCard(2, Suit.Diamonds), new[]
            {
                sut.GetCard(3, Suit.Hearts),
                sut.GetCard(4, Suit.Diamonds),
                sut.GetCard(5, Suit.Hearts),
                sut.GetCard(6, Suit.Diamonds),
                sut.GetCard(7, Suit.Hearts)
            });

            result = sut.Play(1, sut.GetCard(3, Suit.Diamonds));
            VerifyPlayer1Play(result, 10, sut.GetCard(3, Suit.Diamonds), new[]
            {
                sut.GetCard(4, Suit.Hearts),
                sut.GetCard(5, Suit.Diamonds),
                sut.GetCard(6, Suit.Hearts),
                sut.GetCard(7, Suit.Diamonds)
            });

            result = sut.Play(2, sut.GetCard(3, Suit.Hearts));
            VerifyPlayer2Play(result, 11, sut.GetCard(3, Suit.Hearts), new[]
            {
                sut.GetCard(4, Suit.Diamonds),
                sut.GetCard(5, Suit.Hearts),
                sut.GetCard(6, Suit.Diamonds),
                sut.GetCard(7, Suit.Hearts)
            });

            result = sut.Play(1, sut.GetCard(4, Suit.Hearts));
            VerifyPlayer1Play(result, 12, sut.GetCard(4, Suit.Hearts), new[]
            {
                sut.GetCard(5, Suit.Diamonds),
                sut.GetCard(6, Suit.Hearts),
                sut.GetCard(7, Suit.Diamonds)
            });

            result = sut.Play(2, sut.GetCard(4, Suit.Diamonds));
            VerifyPlayer2Play(result, 13, sut.GetCard(4, Suit.Diamonds), new[]
            {
                sut.GetCard(5, Suit.Hearts),
                sut.GetCard(6, Suit.Diamonds),
                sut.GetCard(7, Suit.Hearts)
            });

            result = sut.Play(1, sut.GetCard(5, Suit.Diamonds));
            VerifyPlayer1Play(result, 14, sut.GetCard(5, Suit.Diamonds), new[]
            {
                sut.GetCard(6, Suit.Hearts),
                sut.GetCard(7, Suit.Diamonds)
            });

            result = sut.Play(2, sut.GetCard(5, Suit.Hearts));
            VerifyPlayer2Play(result, 15, sut.GetCard(5, Suit.Hearts), new[]
            {
                sut.GetCard(6, Suit.Diamonds),
                sut.GetCard(7, Suit.Hearts)
            });


            result = sut.Play(1, sut.GetCard(6, Suit.Hearts));
            VerifyPlayer1Play(result, 16, sut.GetCard(6, Suit.Hearts), new[]
            {
                sut.GetCard(7, Suit.Diamonds)
            });

            result = sut.Play(2, sut.GetCard(6, Suit.Diamonds));
            VerifyPlayer2Play(result, 17, sut.GetCard(6, Suit.Diamonds), new[]
            {
                sut.GetCard(7, Suit.Hearts)
            });

            result = sut.Play(1, sut.GetCard(7, Suit.Diamonds));
            VerifyPlayer1Won(result, 17, sut.GetCard(7, Suit.Diamonds));
        }

        private void VerifyPlayer1Take(ActionResultWithCard takeResult, int turnNumber, Card takenCard,
            Card[] player1Hand)
        {
            takeResult.IsSuccess.Should().BeTrue();
            takeResult.Card.Should().Be(takenCard);
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().Equal(player1Hand);
        }

        private void VerifyPlayer2Take(ActionResultWithCard takeResult, int turnNumber, Card takenCard,
            Card[] player2Hand)
        {
            takeResult.IsSuccess.Should().BeTrue();
            takeResult.Card.Should().Be(takenCard);
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.Player2.Hand.Cards.Should().Equal(player2Hand);
        }

        private void VerifyPlayer1Won(ActionResult result, int turnNumber, Card discardCard)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().BeEmpty();
            sut.GameState.CurrentTurn.HasWinner.Should().BeTrue();
            sut.GameState.CurrentTurn.Winner.Should().Be(1);
        }

        private void VerifyPlayer1Play(ActionResult result, int turnNumber, Card discardCard, Card[] player1Hand)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Player1.Hand.Cards.Should().Equal(player1Hand);
        }

        private void VerifyPlayer2Play(ActionResult result, int turnNumber, Card discardCard, Card[] player2Hand)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Player2.Hand.Cards.Should().Equal(player2Hand);
        }
    }
}
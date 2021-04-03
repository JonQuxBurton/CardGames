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

            sut = CreateSut(player1Hand, player2Hand, discardCard, stockPile);

            sut.ChooseStartingPlayer(1);
            sut.Deal();

            sut.GameState.CurrentTurn.TurnNumber.Should().Be(1);
            sut.GameState.CurrentBoard.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            sut.GameState.CurrentBoard.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.StockPile.Cards.Should().Equal(stockPile.Cards);

            var result = sut.Play(1, Card(1, Suit.Diamonds));
            VerifyPlayer1Play(result, 2, Card(1, Suit.Diamonds), new[]
            {
                Card(2, Suit.Hearts),
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(1, Suit.Spades));
            VerifyPlayer2Play(result, 3, Card(1, Suit.Spades), new[]
            {
                Card(2, Suit.Diamonds),
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            var takeResult = sut.Take(1);
            VerifyPlayer1Take(takeResult, 4, Card(8, Suit.Clubs), new[]
            {
                Card(2, Suit.Hearts),
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds),
                Card(8, Suit.Clubs)
            });

            takeResult = sut.Take(2);
            VerifyPlayer2Take(takeResult, 5, Card(8, Suit.Spades), new[]
            {
                Card(2, Suit.Diamonds),
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts),
                Card(8, Suit.Spades)
            });

            result = sut.Play(1, Card(8, Suit.Clubs));
            VerifyPlayer1Play(result, 5, Card(8, Suit.Clubs), new[]
            {
                Card(2, Suit.Hearts),
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.SelectSuit(1, Suit.Spades);

            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(6);
            sut.GameState.CurrentTurn.SelectedSuit.Should().Be(Suit.Spades);

            result = sut.Play(2, Card(8, Suit.Spades));
            VerifyPlayer2Play(result, 6, Card(8, Suit.Spades), new[]
            {
                Card(2, Suit.Diamonds),
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.SelectSuit(2, Suit.Hearts);

            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(7);
            sut.GameState.CurrentTurn.SelectedSuit.Should().Be(Suit.Hearts);

            result = sut.Play(1, Card(2, Suit.Hearts));
            VerifyPlayer1Play(result, 8, Card(2, Suit.Hearts), new[]
            {
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(2, Suit.Diamonds));
            VerifyPlayer2Play(result, 9, Card(2, Suit.Diamonds), new[]
            {
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(3, Suit.Diamonds));
            VerifyPlayer1Play(result, 10, Card(3, Suit.Diamonds), new[]
            {
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(3, Suit.Hearts));
            VerifyPlayer2Play(result, 11, Card(3, Suit.Hearts), new[]
            {
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(4, Suit.Hearts));
            VerifyPlayer1Play(result, 12, Card(4, Suit.Hearts), new[]
            {
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(4, Suit.Diamonds));
            VerifyPlayer2Play(result, 13, Card(4, Suit.Diamonds), new[]
            {
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(5, Suit.Diamonds));
            VerifyPlayer1Play(result, 14, Card(5, Suit.Diamonds), new[]
            {
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(5, Suit.Hearts));
            VerifyPlayer2Play(result, 15, Card(5, Suit.Hearts), new[]
            {
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });


            result = sut.Play(1, Card(6, Suit.Hearts));
            VerifyPlayer1Play(result, 16, Card(6, Suit.Hearts), new[]
            {
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(6, Suit.Diamonds));
            VerifyPlayer2Play(result, 17, Card(6, Suit.Diamonds), new[]
            {
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(7, Suit.Diamonds));
            VerifyPlayer1Won(result, 17, Card(7, Suit.Diamonds));
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

            sut = CreateSut(player1Hand, player2Hand, discardCard, stockPile);
            
            sut.ChooseStartingPlayer(2);
            sut.Deal();

            sut.GameState.CurrentTurn.TurnNumber.Should().Be(1);
            sut.GameState.CurrentBoard.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            sut.GameState.CurrentBoard.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.StockPile.Cards.Should().Equal(stockPile.Cards);

            var result = sut.Play(2, Card(1, Suit.Spades));
            VerifyPlayer2Play(result, 2, Card(1, Suit.Spades), new[]
            {
                    Card(2, Suit.Diamonds),
                    Card(3, Suit.Hearts),
                    Card(4, Suit.Diamonds),
                    Card(5, Suit.Hearts),
                    Card(6, Suit.Diamonds),
                    Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(1, Suit.Diamonds));
            VerifyPlayer1Play(result, 3, Card(1, Suit.Diamonds), new[]
            {
                    Card(2, Suit.Hearts),
                    Card(3, Suit.Diamonds),
                    Card(4, Suit.Hearts),
                    Card(5, Suit.Diamonds),
                    Card(6, Suit.Hearts),
                    Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(2, Suit.Diamonds));
            VerifyPlayer2Play(result, 4, Card(2, Suit.Diamonds), new[]
            {
                Card(3, Suit.Hearts),
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(2, Suit.Hearts));
            VerifyPlayer1Play(result, 5, Card(2, Suit.Hearts), new[]
            {
                Card(3, Suit.Diamonds),
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(3, Suit.Hearts));
            VerifyPlayer2Play(result, 6, Card(3, Suit.Hearts), new[]
            {
                Card(4, Suit.Diamonds),
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(3, Suit.Diamonds));
            VerifyPlayer1Play(result, 7, Card(3, Suit.Diamonds), new[]
            {
                Card(4, Suit.Hearts),
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });
            
            result = sut.Play(2, Card(4, Suit.Diamonds));
            VerifyPlayer2Play(result, 8, Card(4, Suit.Diamonds), new[]
            {
                Card(5, Suit.Hearts),
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(4, Suit.Hearts));
            VerifyPlayer1Play(result, 9, Card(4, Suit.Hearts), new[]
            {
                Card(5, Suit.Diamonds),
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(5, Suit.Hearts));
            VerifyPlayer2Play(result, 10, Card(5, Suit.Hearts), new[]
            {
                Card(6, Suit.Diamonds),
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(5, Suit.Diamonds));
            VerifyPlayer1Play(result,11, Card(5, Suit.Diamonds), new[]
            {
                Card(6, Suit.Hearts),
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(6, Suit.Diamonds));
            VerifyPlayer2Play(result, 12, Card(6, Suit.Diamonds), new[]
            {
                Card(7, Suit.Hearts)
            });

            result = sut.Play(1, Card(6, Suit.Hearts));
            VerifyPlayer1Play(result, 13, Card(6, Suit.Hearts), new[]
            {
                Card(7, Suit.Diamonds)
            });

            result = sut.Play(2, Card(7, Suit.Hearts));
            VerifyPlayer2Won(result, 13, Card(7, Suit.Hearts));
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
            var dealer = new Dealer(rules);

            sut = new Game(rules, shuffler, dealer, deck, players);
            
            sut.ChooseStartingPlayer(3);
            sut.Deal();

            sut.GameState.CurrentTurn.TurnNumber.Should().Be(1);
            sut.GameState.CurrentBoard.Players[0].Hand.Cards.Should().Equal(player1Hand.Cards);
            sut.GameState.CurrentBoard.Players[1].Hand.Cards.Should().Equal(player2Hand.Cards);
            sut.GameState.CurrentBoard.Players[2].Hand.Cards.Should().Equal(player3Hand.Cards);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.StockPile.Cards.Should().Equal(stockPile.Cards);

            var result = sut.Play(3, Card(1, Suit.Spades));
            VerifyPlayer3Play(result, 2, Card(1, Suit.Spades), new[]
            {
                new Card(3, Suit.Spades),
                new Card(5, Suit.Spades),
                new Card(7, Suit.Spades),
                new Card(9, Suit.Spades)
            });

            result = sut.Play(1, Card(1, Suit.Clubs));
            VerifyPlayer1Play(result, 3, Card(1, Suit.Clubs), new[]
            {
                new Card(3, Suit.Diamonds),
                new Card(5, Suit.Clubs),
                new Card(7, Suit.Diamonds),
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(2, Card(3, Suit.Clubs));
            VerifyPlayer2Play(result, 4, Card(3, Suit.Clubs), new[]
            {
                new Card(5, Suit.Diamonds),
                new Card(7, Suit.Clubs),
                new Card(9, Suit.Diamonds),
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(3, Card(3, Suit.Spades));
            VerifyPlayer3Play(result, 5, Card(3, Suit.Spades), new[]
            {
                new Card(5, Suit.Spades),
                new Card(7, Suit.Spades),
                new Card(9, Suit.Spades)
            });

            result = sut.Play(1, Card(3, Suit.Diamonds));
            VerifyPlayer1Play(result, 6, Card(3, Suit.Diamonds), new[]
            {
                new Card(5, Suit.Clubs),
                new Card(7, Suit.Diamonds),
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(2, Card(5, Suit.Diamonds));
            VerifyPlayer2Play(result, 7, Card(5, Suit.Diamonds), new[]
            {
                new Card(7, Suit.Clubs),
                new Card(9, Suit.Diamonds),
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(3, Card(5, Suit.Spades));
            VerifyPlayer3Play(result, 8, Card(5, Suit.Spades), new[]
            {
                new Card(7, Suit.Spades),
                new Card(9, Suit.Spades)
            });

            result = sut.Play(1, Card(5, Suit.Clubs));
            VerifyPlayer1Play(result, 9, Card(5, Suit.Clubs), new[]
            {
                new Card(7, Suit.Diamonds),
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(2, Card(7, Suit.Clubs));
            VerifyPlayer2Play(result, 10, Card(7, Suit.Clubs), new[]
            {
                new Card(9, Suit.Diamonds),
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(3, Card(7, Suit.Spades));
            VerifyPlayer3Play(result, 11, Card(7, Suit.Spades), new[]
            {
                new Card(9, Suit.Spades)
            });

            result = sut.Play(1, Card(7, Suit.Diamonds));
            VerifyPlayer1Play(result, 12, Card(7, Suit.Diamonds), new[]
            {
                new Card(9, Suit.Clubs)
            });

            result = sut.Play(2, Card(9, Suit.Diamonds));
            VerifyPlayer2Play(result, 13, Card(9, Suit.Diamonds), new[]
            {
                new Card(11, Suit.Clubs)
            });

            result = sut.Play(3, Card(9, Suit.Spades));
            VerifyPlayer3Won(result, 13, Card(9, Suit.Spades));
        }

        private Game CreateSut(CardCollection player1Hand, CardCollection player2Hand, Card discardCard,
            CardCollection stockPile)
        {
            IDeckBuilder deckBuilder = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand);
            var deck = deckBuilder.Build();

            var players = new[] {new Player(1, "Alice"), new Player(2, "Bob")};
            var rules = new Rules(7);
            var shuffler = new DummyShuffler();
            var dealer = new Dealer(rules);

            return new Game(rules, shuffler, dealer, deck, players);
        }

        private Card Card(int rank, Suit suit)
        {
            return sut.GetCard(rank, suit);
        }

        private void VerifyPlayer1Take(ActionResultWithCard takeResult, int turnNumber, Card takenCard,
            Card[] player1Hand)
        {
            takeResult.IsSuccess.Should().BeTrue();
            takeResult.Card.Should().Be(takenCard);
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.Players[0].Hand.Cards.Should().Equal(player1Hand);
        }

        private void VerifyPlayer2Take(ActionResultWithCard takeResult, int turnNumber, Card takenCard,
            Card[] player2Hand)
        {
            takeResult.IsSuccess.Should().BeTrue();
            takeResult.Card.Should().Be(takenCard);
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.Players[1].Hand.Cards.Should().Equal(player2Hand);
        }

        private void VerifyPlayer1Won(ActionResult result, int turnNumber, Card discardCard)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Players[0].Hand.Cards.Should().BeEmpty();
            sut.GameState.CurrentTurn.HasWinner.Should().BeTrue();
            sut.GameState.CurrentTurn.Winner.Number.Should().Be(1);
        }
        private void VerifyPlayer2Won(ActionResult result, int turnNumber, Card discardCard)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Players[1].Hand.Cards.Should().BeEmpty();
            sut.GameState.CurrentTurn.HasWinner.Should().BeTrue();
            sut.GameState.CurrentTurn.Winner.Number.Should().Be(2);
        }

        private void VerifyPlayer3Won(ActionResult result, int turnNumber, Card discardCard)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Players[2].Hand.Cards.Should().BeEmpty();
            sut.GameState.CurrentTurn.HasWinner.Should().BeTrue();
            sut.GameState.CurrentTurn.Winner.Number.Should().Be(3);
        }

        private void VerifyPlayer1Play(ActionResult result, int turnNumber, Card discardCard, Card[] player1Hand)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Players[0].Hand.Cards.Should().Equal(player1Hand);
        }

        private void VerifyPlayer2Play(ActionResult result, int turnNumber, Card discardCard, Card[] player2Hand)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Players[1].Hand.Cards.Should().Equal(player2Hand);
        }

        private void VerifyPlayer3Play(ActionResult result, int turnNumber, Card discardCard, Card[] player3Hand)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentBoard.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentBoard.Players[2].Hand.Cards.Should().Equal(player3Hand);
        }
    }
}
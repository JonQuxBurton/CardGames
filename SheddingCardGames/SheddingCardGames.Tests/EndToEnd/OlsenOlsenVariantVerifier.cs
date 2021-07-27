using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;

namespace SheddingCardGames.Tests.EndToEnd
{
    public class OlsenOlsenVariantVerifier
    {
        private readonly Game sut;
        private readonly Dictionary<int, CardCollection> playerHands;
        private readonly Card discardCard;
        private readonly CardCollection stockPile;

        public OlsenOlsenVariantVerifier(Game sut, Dictionary<int, CardCollection> playerHands, Card discardCard,
            CardCollection stockPile)
        {
            this.sut = sut;
            this.playerHands = playerHands;
            this.discardCard = discardCard;
            this.stockPile = stockPile;
        }

        public void VerifySetup(int numberOfPlayer)
        {
            sut.GameState.CurrentStateOfTurn.TurnNumber.Should().Be(1);
            for (var i = 0; i < numberOfPlayer; i++)
                sut.GameState.CurrentTable.Players[i].Hand.Cards.Should().Equal(playerHands[i + 1].Cards);

            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(discardCard);
            sut.GameState.CurrentTable.StockPile.Cards.Should().Equal(stockPile.Cards);
        }

        public void VerifyTake(int playerNumber, CommandExecutionResult takeResult, int turnNumber,
            Card takenCard,
            params Card[] hand)
        {
            takeResult.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentStateOfTurn.TakenCard.Should().Be(takenCard);
            sut.GameState.CurrentStateOfTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentTable.Players[playerNumber - 1].Hand.Cards.Should().Equal(hand);
        }

        public void VerifyWon(int playerNumber, CommandExecutionResult result, int turnNumber,
            Card[] cardsPlayed)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentStateOfTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentTable.Players[playerNumber - 1].Hand.Cards.Should().BeEmpty();
            sut.GameState.CurrentStateOfPlay.HasWinner.Should().BeTrue();
            sut.GameState.CurrentStateOfPlay.Winner.Number.Should().Be(playerNumber);
            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardsPlayed.Last());

            var cardsPlayedExceptLast = cardsPlayed.SkipLast(1);
            var cardsAddedToDiscardPile = sut.GameState.CurrentTable.DiscardPile.RestOfCards.Cards.SkipLast(1)
                .Take(cardsPlayedExceptLast.Count());
            cardsAddedToDiscardPile.Should().Equal(cardsPlayedExceptLast.Reverse());
        }

        public void VerifySelectSuit(CommandExecutionResult result, int turnNumber, Suit selectedSuit)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentStateOfTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentStateOfTurn.SelectedSuit.Should().Be(selectedSuit);
        }

        public void VerifyPlayMultiple(int playerNumber, CommandExecutionResult result, int turnNumber,
            Card[] cardsPlayed, params Card[] hand)
        {
            result.IsSuccess.Should().BeTrue();
            sut.GameState.CurrentStateOfTurn.TurnNumber.Should().Be(turnNumber);
            sut.GameState.CurrentTable.Players[playerNumber - 1].Hand.Cards.Should().Equal(hand);
            sut.GameState.CurrentTable.DiscardPile.CardToMatch.Should().Be(cardsPlayed.Last());

            var cardsPlayedExceptLast = cardsPlayed.SkipLast(1);
            var cardsAddedToDiscardPile = sut.GameState.CurrentTable.DiscardPile.RestOfCards.Cards.SkipLast(1)
                .Take(cardsPlayedExceptLast.Count());
            cardsAddedToDiscardPile.Should().Equal(cardsPlayedExceptLast.Reverse());
        }
    }
}
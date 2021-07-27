using System.Collections.Generic;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEights.CrazyEightsRules;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.EndToEnd
{
    namespace OlsenOlsenVariant
    {
        public class PlayAGame
        {
            private readonly Player player1;
            private readonly Player player2;
            private readonly Player player3;
            private readonly Player player4;
            private readonly Dictionary<int, CardCollection> playerHands;
            private Card discardCard;
            private CardCollection stockPile;
            private Game sut;

            public PlayAGame()
            {
                var sampleData = new SampleData();
                player1 = sampleData.Player1;
                player2 = sampleData.Player2;
                player3 = sampleData.Player3;
                player4 = sampleData.Player4;

                playerHands = new Dictionary<int, CardCollection>();
            }

            // Scenarios:
            // PlayMultiple not Eight
            // PlayMultiple Eight, SelectSuit
            // Take, PlayMultiple
            // Take, Take, Take, Pass
            // PlayMultiple not Eight, Won
            [Fact]
            public void SampleGameWithTwoPlayers()
            {
                playerHands[1] = new CardCollection(
                    Card(1, Diamonds),
                    Card(1, Spades),
                    Card(1, Hearts),
                    Card(8, Clubs),
                    Card(13, Clubs),
                    Card(13, Diamonds),
                    Card(13, Spades)
                );
                playerHands[2] = new CardCollection(
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds),
                    Card(6, Diamonds),
                    Card(7, Diamonds),
                    Card(9, Diamonds)
                );
                discardCard = new Card(1, Clubs);
                stockPile = new CardCollection(
                    Card(10, Spades),
                    Card(11, Spades),
                    Card(12, Spades),
                    Card(13, Hearts)
                );

                sut = CreateSut(player1, NumberOfPlayers.Two, new[] {player1, player2},
                    new[] {playerHands[1], playerHands[2]});
                var verifier = new OlsenOlsenVariantVerifier(sut, playerHands, discardCard, stockPile);

                verifier.VerifySetup(playerHands.Count);

                // Turn 1 (Scenario: PlayMultiple not Eight)
                var result =
                    sut.Play(new PlayContext(player1, Cards(Card(1, Diamonds), Card(1, Spades), Card(1, Hearts))));
                verifier.VerifyPlayMultiple(1, result, 2, new[] {Card(1, Diamonds), Card(1, Spades), Card(1, Hearts)},
                    Card(8, Clubs), Card(13, Clubs), Card(13, Diamonds), Card(13, Spades));

                // Turn 2 (Scenario: Take, Take, Take, Pass)
                var takeResult = sut.Take(new TakeContext(player2));
                verifier.VerifyTake(2, takeResult, 2, Card(10, Spades), Card(2, Diamonds), Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds), Card(6, Diamonds), Card(7, Diamonds), Card(9, Diamonds), Card(10, Spades));

                takeResult = sut.Take(new TakeContext(player2, 1));
                verifier.VerifyTake(2, takeResult, 2, Card(11, Spades), Card(2, Diamonds), Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds), Card(6, Diamonds), Card(7, Diamonds), Card(9, Diamonds), Card(10, Spades),
                    Card(11, Spades));

                takeResult = sut.Take(new TakeContext(player2, 2));
                verifier.VerifyTake(2, takeResult, 3, Card(12, Spades), Card(2, Diamonds), Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds), Card(6, Diamonds), Card(7, Diamonds), Card(9, Diamonds), Card(10, Spades),
                    Card(11, Spades), Card(12, Spades));

                // Turn 3 (Scenario: PlayMultiple Eight, SelectSuit)
                result = sut.Play(new PlayContext(player1, Cards(Card(8, Clubs))));
                verifier.VerifyPlayMultiple(1, result, 3, new[] {Card(8, Clubs)}, Card(13, Clubs), Card(13, Diamonds),
                    Card(13, Spades));

                result = sut.SelectSuit(new SelectSuitContext(player1, Hearts));
                verifier.VerifySelectSuit(result, 4, Hearts);

                // Turn 4 (Scenario: Take, PlayMultiple)
                takeResult = sut.Take(new TakeContext(player2));
                verifier.VerifyTake(2, takeResult, 4, Card(13, Hearts), Card(2, Diamonds), Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds), Card(6, Diamonds), Card(7, Diamonds), Card(9, Diamonds), Card(10, Spades),
                    Card(11, Spades), Card(12, Spades), Card(13, Hearts));

                result = sut.Play(new PlayContext(player2, Cards(Card(13, Hearts))));
                verifier.VerifyPlayMultiple(2, result, 5,
                    new[] {Card(13, Hearts)},
                    Card(2, Diamonds), Card(3, Diamonds), Card(4, Diamonds), Card(5, Diamonds), Card(6, Diamonds),
                    Card(7, Diamonds), Card(9, Diamonds), Card(10, Spades), Card(11, Spades), Card(12, Spades));

                // Turn 5 (Scenario: PlayMultiple not Eight, Won)
                result = sut.Play(
                    new PlayContext(player1, Cards(Card(13, Clubs), Card(13, Diamonds), Card(13, Spades))));
                verifier.VerifyWon(1, result, 5, new[] {Card(13, Clubs), Card(13, Diamonds), Card(13, Spades)});
            }

            // Sample Game
            //
            // Discard Card
            // 10 Clubs
            //
            // Order
            // Player 4
            // Player 1
            // Player 2
            // Player 3
            //
            // 1 Clubs
            // 1 Diamonds
            // 1 Spades
            // 1 Hearts
            //
            // 2 Hearts
            // 2 Diamonds
            // 2 Spades
            // 2 Clubs
            //
            // 3 Clubs
            // 3 Diamonds
            // 3 Spades
            // 3 Hearts
            //
            // 4 Hearts
            // 4 Diamonds
            // 4 Spades
            // 4 Clubs
            //
            // 5 Clubs
            // Player 4 WON
            [Fact]
            public void SampleGameWithFourPlayers()
            {
                playerHands[1] = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds)
                );
                playerHands[2] = new CardCollection(
                    Card(1, Spades),
                    Card(2, Spades),
                    Card(3, Spades),
                    Card(4, Spades),
                    Card(5, Spades)
                );
                playerHands[3] = new CardCollection(
                    Card(1, Hearts),
                    Card(2, Clubs),
                    Card(3, Hearts),
                    Card(4, Clubs),
                    Card(5, Hearts)
                );
                playerHands[4] = new CardCollection(
                    Card(1, Clubs),
                    Card(2, Hearts),
                    Card(3, Clubs),
                    Card(4, Hearts),
                    Card(5, Clubs)
                );
                discardCard = new Card(10, Clubs);
                stockPile = new CardCollection(
                    Card(8, Clubs),
                    Card(8, Spades)
                );

                sut = CreateSut(player4, NumberOfPlayers.Four, new[] {player1, player2, player3, player4},
                    new[] {playerHands[1], playerHands[2], playerHands[3], playerHands[4]});
                var verifier = new OlsenOlsenVariantVerifier(sut, playerHands, discardCard, stockPile);

                verifier.VerifySetup(playerHands.Count);

                // Turn 1
                var result = sut.Play(new PlayContext(player4, Card(1, Clubs)));
                verifier.VerifyPlayMultiple(4, result, 2,
                    new[] {Card(1, Clubs)},
                    Card(2, Hearts), Card(3, Clubs), Card(4, Hearts), Card(5, Clubs));

                // Turn 2
                result = sut.Play(new PlayContext(player1, Card(1, Diamonds)));
                verifier.VerifyPlayMultiple(1, result, 3, new[] {Card(1, Diamonds)}, Card(2, Diamonds),
                    Card(3, Diamonds),
                    Card(4, Diamonds), Card(5, Diamonds));

                // Turn 3
                result = sut.Play(new PlayContext(player2, Card(1, Spades)));
                verifier.VerifyPlayMultiple(2, result, 4, new[] {Card(1, Spades)}, Card(2, Spades), Card(3, Spades),
                    Card(4, Spades), Card(5, Spades));

                // Turn 4
                result = sut.Play(new PlayContext(player3, Card(1, Hearts)));
                verifier.VerifyPlayMultiple(3, result, 5, new[] {Card(1, Hearts)}, Card(2, Clubs), Card(3, Hearts),
                    Card(4, Clubs), Card(5, Hearts));

                // Turn 5
                result = sut.Play(new PlayContext(player4, Card(2, Hearts)));
                verifier.VerifyPlayMultiple(4, result, 6, new[] {Card(2, Hearts)}, Card(3, Clubs), Card(4, Hearts),
                    Card(5, Clubs));

                // Turn 6
                result = sut.Play(new PlayContext(player1, Card(2, Diamonds)));
                verifier.VerifyPlayMultiple(1, result, 7, new[] {Card(2, Diamonds)}, Card(3, Diamonds),
                    Card(4, Diamonds),
                    Card(5, Diamonds));

                // Turn 7
                result = sut.Play(new PlayContext(player2, Card(2, Spades)));
                verifier.VerifyPlayMultiple(2, result, 8, new[] {Card(2, Spades)}, Card(3, Spades), Card(4, Spades),
                    Card(5, Spades));

                // Turn 8
                result = sut.Play(new PlayContext(player3, Card(2, Clubs)));
                verifier.VerifyPlayMultiple(3, result, 9, new[] {Card(2, Clubs)}, Card(3, Hearts), Card(4, Clubs),
                    Card(5, Hearts));

                // Turn 9
                result = sut.Play(new PlayContext(player4, Card(3, Clubs)));
                verifier.VerifyPlayMultiple(4, result, 10, new[] {Card(3, Clubs)}, Card(4, Hearts), Card(5, Clubs));

                // Turn 10
                result = sut.Play(new PlayContext(player1, Card(3, Diamonds)));
                verifier.VerifyPlayMultiple(1, result, 11, new[] {Card(3, Diamonds)}, Card(4, Diamonds),
                    Card(5, Diamonds));

                // Turn 11
                result = sut.Play(new PlayContext(player2, Card(3, Spades)));
                verifier.VerifyPlayMultiple(2, result, 12, new[] {Card(3, Spades)}, Card(4, Spades), Card(5, Spades));

                // Turn 12
                result = sut.Play(new PlayContext(player3, Card(3, Hearts)));
                verifier.VerifyPlayMultiple(3, result, 13, new[] {Card(3, Hearts)}, Card(4, Clubs), Card(5, Hearts));

                // Turn 13
                result = sut.Play(new PlayContext(player4, Card(4, Hearts)));
                verifier.VerifyPlayMultiple(4, result, 14, new[] {Card(4, Hearts)}, Card(5, Clubs));

                // Turn 14
                result = sut.Play(new PlayContext(player1, Card(4, Diamonds)));
                verifier.VerifyPlayMultiple(1, result, 15, new[] {Card(4, Diamonds)}, Card(5, Diamonds));

                // Turn 15
                result = sut.Play(new PlayContext(player2, Card(4, Spades)));
                verifier.VerifyPlayMultiple(2, result, 16, new[] {Card(4, Spades)}, Card(5, Spades));

                // Turn 16
                result = sut.Play(new PlayContext(player3, Card(4, Clubs)));
                verifier.VerifyPlayMultiple(3, result, 17, new[] {Card(4, Clubs)}, Card(5, Hearts));

                // Turn 17
                result = sut.Play(new PlayContext(player4, Card(5, Clubs)));
                verifier.VerifyWon(4, result, 17, new[] {Card(5, Clubs)});
            }

            private Game CreateSut(Player startingPlayer, NumberOfPlayers numberOfPlayers, Player[] players,
                CardCollection[] playerHands)
            {
                IDeckBuilder deckBuilder = new SpecificDeckBuilder(discardCard, stockPile, playerHands);
                var deck = deckBuilder.Build();

                var rules = new OlsenOlsenVariantRules(numberOfPlayers);
                var shuffler = new DummyShuffler();
                var dummyPlayerChooser = new DummyPlayerChooser(startingPlayer);

                var game = new Game(
                    new Variant(VariantName.OlsenOlsen,
                        new OlsenOlsenVariantCommandFactory(rules, shuffler, dummyPlayerChooser)), players);

                game.ChooseStartingPlayer(new ChooseStartingPlayerContext());
                game.Deal(new DealContext(deck));

                return game;
            }
        }
    }
}
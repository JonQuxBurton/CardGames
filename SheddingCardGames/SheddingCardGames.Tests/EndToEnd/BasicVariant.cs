using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Xunit;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.CrazyEightsRules;
using static SheddingCardGames.Domain.Suit;

namespace SheddingCardGames.Tests.EndToEnd
{
    namespace BasicVariant
    {
        public partial class PlayAGame
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
            // PlaySingle not Eight
            // PlaySingle Eight, SelectSuit
            // Take
            // PlaySingle not Eight, Won
            [Fact]
            public void SampleGameWithTwoPlayers()
            {
                playerHands[1] = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Hearts),
                    Card(3, Diamonds),
                    Card(4, Hearts),
                    Card(5, Diamonds),
                    Card(6, Hearts),
                    Card(7, Diamonds)
                );
                playerHands[2] = new CardCollection(
                    Card(1, Spades),
                    Card(2, Diamonds),
                    Card(3, Hearts),
                    Card(4, Diamonds),
                    Card(5, Hearts),
                    Card(6, Diamonds),
                    Card(7, Hearts)
                );
                discardCard = new Card(1, Clubs);
                stockPile = new CardCollection(
                    Card(8, Clubs),
                    Card(8, Spades)
                );

                sut = CreateSut(player1, NumberOfPlayers.Two, new[] {player1, player2},
                    new[] {playerHands[1], playerHands[2]});
                var verifier = new BasicVariantVerifier(sut, playerHands, discardCard, stockPile);

                verifier.VerifySetup(playerHands.Count);

                // Turn 1 (Scenario: PlaySingle not Eight)
                var result = sut.Play(new PlayContext(player1, Card(1, Diamonds)));
                verifier.VerifyPlay(1, result, 2, Card(1, Diamonds), Card(2, Hearts), Card(3, Diamonds), Card(4, Hearts),
                    Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds));

                // Turn 2
                result = sut.Play(new PlayContext(player2, Card(1, Spades)));
                verifier.VerifyPlay(2, result, 3, Card(1, Spades), Card(2, Diamonds), Card(3, Hearts), Card(4, Diamonds),
                    Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts));

                // Turn 3 (Scenario: Take)
                var takeResult = sut.Take(new TakeContext(player1));
                verifier.VerifyTake(1, takeResult, 4, Card(8, Clubs), Card(2, Hearts), Card(3, Diamonds), Card(4, Hearts),
                    Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds), Card(8, Clubs));

                // Turn 4
                takeResult = sut.Take(new TakeContext(player2));
                verifier.VerifyTake(2, takeResult, 5, Card(8, Spades), Card(2, Diamonds), Card(3, Hearts),
                    Card(4, Diamonds), Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts), Card(8, Spades));

                // Turn 5 (Scenario: PlaySingle Eight, SelectSuit)
                result = sut.Play(new PlayContext(player1, Card(8, Clubs)));
                verifier.VerifyPlay(1, result, 5, Card(8, Clubs), Card(2, Hearts), Card(3, Diamonds), Card(4, Hearts),
                    Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds));

                result = sut.SelectSuit(new SelectSuitContext(player1, Spades));
                verifier.VerifySelectSuit(result, 6, Spades);

                // Turn 6
                result = sut.Play(new PlayContext(player2, Card(8, Spades)));
                verifier.VerifyPlay(2, result, 6, Card(8, Spades), Card(2, Diamonds), Card(3, Hearts), Card(4, Diamonds),
                    Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts));

                result = sut.SelectSuit(new SelectSuitContext(player2, Hearts));
                verifier.VerifySelectSuit(result, 7, Hearts);

                // Turn 7
                result = sut.Play(new PlayContext(player1, Card(2, Hearts)));
                verifier.VerifyPlay(1, result, 8, Card(2, Hearts), Card(3, Diamonds), Card(4, Hearts), Card(5, Diamonds),
                    Card(6, Hearts), Card(7, Diamonds));

                // Turn 8
                result = sut.Play(new PlayContext(player2, Card(2, Diamonds)));
                verifier.VerifyPlay(2, result, 9, Card(2, Diamonds), Card(3, Hearts), Card(4, Diamonds), Card(5, Hearts),
                    Card(6, Diamonds), Card(7, Hearts));

                // Turn 9
                result = sut.Play(new PlayContext(player1, Card(3, Diamonds)));
                verifier.VerifyPlay(1, result, 10, Card(3, Diamonds), Card(4, Hearts), Card(5, Diamonds), Card(6, Hearts),
                    Card(7, Diamonds));

                // Turn 10 
                result = sut.Play(new PlayContext(player2, Card(3, Hearts)));
                verifier.VerifyPlay(2, result, 11, Card(3, Hearts), Card(4, Diamonds), Card(5, Hearts), Card(6, Diamonds),
                    Card(7, Hearts));

                // Turn 11
                result = sut.Play(new PlayContext(player1, Card(4, Hearts)));
                verifier.VerifyPlay(1, result, 12, Card(4, Hearts), Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds));

                // Turn 12
                result = sut.Play(new PlayContext(player2, Card(4, Diamonds)));
                verifier.VerifyPlay(2, result, 13, Card(4, Diamonds), Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts));

                // Turn 13
                result = sut.Play(new PlayContext(player1, Card(5, Diamonds)));
                verifier.VerifyPlay(1, result, 14, Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds));

                // Turn 14
                result = sut.Play(new PlayContext(player2, Card(5, Hearts)));
                verifier.VerifyPlay(2, result, 15, Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts));

                // Turn 15
                result = sut.Play(new PlayContext(player1, Card(6, Hearts)));
                verifier.VerifyPlay(1, result, 16, Card(6, Hearts), Card(7, Diamonds));

                // Turn 16
                result = sut.Play(new PlayContext(player2, Card(6, Diamonds)));
                verifier.VerifyPlay(2, result, 17, Card(6, Diamonds), Card(7, Hearts));

                // Turn 17 (Scenario: PlaySingle not Eight, Won)
                result = sut.Play(new PlayContext(player1, Card(7, Diamonds)));
                verifier.VerifyWon(1, result, 17, Card(7, Diamonds));
            }

            [Fact]
            public void WithTwoPlayers_WherePlayerTwoWins()
            {
                playerHands[1] = new CardCollection(
                    Card(1, Diamonds),
                    Card(2, Hearts),
                    Card(3, Diamonds),
                    Card(4, Hearts),
                    Card(5, Diamonds),
                    Card(6, Hearts),
                    Card(7, Diamonds)
                );
                playerHands[2] = new CardCollection(
                    Card(1, Spades),
                    Card(2, Diamonds),
                    Card(3, Hearts),
                    Card(4, Diamonds),
                    Card(5, Hearts),
                    Card(6, Diamonds),
                    Card(7, Hearts)
                );
                discardCard = new Card(1, Clubs);
                stockPile = new CardCollection(
                    Card(8, Clubs),
                    Card(8, Spades)
                );

                sut = CreateSut(player2, NumberOfPlayers.Two, new[] {player1, player2},
                    new[] {playerHands[1], playerHands[2]});
                var verifier = new BasicVariantVerifier(sut, playerHands, discardCard, stockPile);
                verifier.VerifySetup(playerHands.Count);

                // Turn 1
                var result = sut.Play(new PlayContext(player2, Card(1, Spades)));
                verifier.VerifyPlay(2, result, 2, Card(1, Spades), Card(2, Diamonds), Card(3, Hearts), Card(4, Diamonds),
                    Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts));

                // Turn 2
                result = sut.Play(new PlayContext(player1, Card(1, Diamonds)));
                verifier.VerifyPlay(1, result, 3, Card(1, Diamonds), Card(2, Hearts), Card(3, Diamonds), Card(4, Hearts),
                    Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds));

                // Turn 3
                result = sut.Play(new PlayContext(player2, Card(2, Diamonds)));
                verifier.VerifyPlay(2, result, 4, Card(2, Diamonds), Card(3, Hearts), Card(4, Diamonds), Card(5, Hearts),
                    Card(6, Diamonds), Card(7, Hearts));

                // Turn 4
                result = sut.Play(new PlayContext(player1, Card(2, Hearts)));
                verifier.VerifyPlay(1, result, 5, Card(2, Hearts), Card(3, Diamonds), Card(4, Hearts), Card(5, Diamonds),
                    Card(6, Hearts), Card(7, Diamonds));

                // Turn 5
                result = sut.Play(new PlayContext(player2, Card(3, Hearts)));
                verifier.VerifyPlay(2, result, 6, Card(3, Hearts), Card(4, Diamonds), Card(5, Hearts), Card(6, Diamonds),
                    Card(7, Hearts));

                // Turn 6
                result = sut.Play(new PlayContext(player1, Card(3, Diamonds)));
                verifier.VerifyPlay(1, result, 7, Card(3, Diamonds), Card(4, Hearts), Card(5, Diamonds), Card(6, Hearts),
                    Card(7, Diamonds));

                // Turn 7
                result = sut.Play(new PlayContext(player2, Card(4, Diamonds)));
                verifier.VerifyPlay(2, result, 8, Card(4, Diamonds), Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts));

                // Turn 8
                result = sut.Play(new PlayContext(player1, Card(4, Hearts)));
                verifier.VerifyPlay(1, result, 9, Card(4, Hearts), Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds));

                // Turn 9
                result = sut.Play(new PlayContext(player2, Card(5, Hearts)));
                verifier.VerifyPlay(2, result, 10, Card(5, Hearts), Card(6, Diamonds), Card(7, Hearts));

                // Turn 10
                result = sut.Play(new PlayContext(player1, Card(5, Diamonds)));
                verifier.VerifyPlay(1, result, 11, Card(5, Diamonds), Card(6, Hearts), Card(7, Diamonds));

                // Turn 11
                result = sut.Play(new PlayContext(player2, Card(6, Diamonds)));
                verifier.VerifyPlay(2, result, 12, Card(6, Diamonds), Card(7, Hearts));

                // Turn 12
                result = sut.Play(new PlayContext(player1, Card(6, Hearts)));
                verifier.VerifyPlay(1, result, 13, Card(6, Hearts), Card(7, Diamonds));

                // Turn 13
                result = sut.Play(new PlayContext(player2, Card(7, Hearts)));
                verifier.VerifyWon(2, result, 13, Card(7, Hearts));
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
                var verifier = new BasicVariantVerifier(sut, playerHands, discardCard, stockPile);
                verifier.VerifySetup(playerHands.Count);

                // Turn 1
                var result = sut.Play(new PlayContext(player4, Card(1, Clubs)));
                verifier.VerifyPlay(4, result, 2, Card(1, Clubs), Card(2, Hearts), Card(3, Clubs), Card(4, Hearts),
                    Card(5, Clubs));

                // Turn 2
                result = sut.Play(new PlayContext(player1, Card(1, Diamonds)));
                verifier.VerifyPlay(1, result, 3, Card(1, Diamonds), Card(2, Diamonds), Card(3, Diamonds), Card(4, Diamonds),
                    Card(5, Diamonds));

                // Turn 3
                result = sut.Play(new PlayContext(player2, Card(1, Spades)));
                verifier.VerifyPlay(2, result, 4, Card(1, Spades), Card(2, Spades), Card(3, Spades), Card(4, Spades),
                    Card(5, Spades));

                // Turn 4
                result = sut.Play(new PlayContext(player3, Card(1, Hearts)));
                verifier.VerifyPlay(3, result, 5, Card(1, Hearts), Card(2, Clubs), Card(3, Hearts), Card(4, Clubs),
                    Card(5, Hearts));

                // Turn 5
                result = sut.Play(new PlayContext(player4, Card(2, Hearts)));
                verifier.VerifyPlay(4, result, 6, Card(2, Hearts), Card(3, Clubs), Card(4, Hearts), Card(5, Clubs));

                // Turn 6
                result = sut.Play(new PlayContext(player1, Card(2, Diamonds)));
                verifier.VerifyPlay(1, result, 7, Card(2, Diamonds), Card(3, Diamonds), Card(4, Diamonds), Card(5, Diamonds));

                // Turn 7
                result = sut.Play(new PlayContext(player2, Card(2, Spades)));
                verifier.VerifyPlay(2, result, 8, Card(2, Spades), Card(3, Spades), Card(4, Spades), Card(5, Spades));

                // Turn 8
                result = sut.Play(new PlayContext(player3, Card(2, Clubs)));
                verifier.VerifyPlay(3, result, 9, Card(2, Clubs), Card(3, Hearts), Card(4, Clubs), Card(5, Hearts));

                // Turn 9
                result = sut.Play(new PlayContext(player4, Card(3, Clubs)));
                verifier.VerifyPlay(4, result, 10, Card(3, Clubs), Card(4, Hearts), Card(5, Clubs));

                // Turn 10
                result = sut.Play(new PlayContext(player1, Card(3, Diamonds)));
                verifier.VerifyPlay(1, result, 11, Card(3, Diamonds), Card(4, Diamonds), Card(5, Diamonds));

                // Turn 11
                result = sut.Play(new PlayContext(player2, Card(3, Spades)));
                verifier.VerifyPlay(2, result, 12, Card(3, Spades), Card(4, Spades), Card(5, Spades));

                // Turn 12
                result = sut.Play(new PlayContext(player3, Card(3, Hearts)));
                verifier.VerifyPlay(3, result, 13, Card(3, Hearts), Card(4, Clubs), Card(5, Hearts));

                // Turn 13
                result = sut.Play(new PlayContext(player4, Card(4, Hearts)));
                verifier.VerifyPlay(4, result, 14, Card(4, Hearts), Card(5, Clubs));

                // Turn 14
                result = sut.Play(new PlayContext(player1, Card(4, Diamonds)));
                verifier.VerifyPlay(1, result, 15, Card(4, Diamonds), Card(5, Diamonds));

                // Turn 15
                result = sut.Play(new PlayContext(player2, Card(4, Spades)));
                verifier.VerifyPlay(2, result, 16, Card(4, Spades), Card(5, Spades));

                // Turn 16
                result = sut.Play(new PlayContext(player3, Card(4, Clubs)));
                verifier.VerifyPlay(3, result, 17, Card(4, Clubs), Card(5, Hearts));

                // Turn 17
                result = sut.Play(new PlayContext(player4, Card(5, Clubs)));
                verifier.VerifyWon(4, result, 17, Card(5, Clubs));
            }

            private Game CreateSut(Player startingPlayer, NumberOfPlayers numberOfPlayers, Player[] players,
                CardCollection[] playerHands)
            {
                IDeckBuilder deckBuilder = new SpecificDeckBuilder(discardCard, stockPile, playerHands);
                var deck = deckBuilder.Build();

                var rules = new BasicVariantRules(numberOfPlayers);
                var shuffler = new DummyShuffler();
                var dummyPlayerChooser = new DummyPlayerChooser(startingPlayer);

                var game = new Game(
                    new Variant(VariantName.Basic,
                        new BasicVariantCommandFactory(rules, shuffler, dummyPlayerChooser)), players);

                game.ChooseStartingPlayer(new ChooseStartingPlayerContext());
                game.Deal(new DealContext(deck));

                return game;
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.CrazyEights;
using SheddingCardGames.Domain.Whist;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CardsUtils;
using static SheddingCardGames.Domain.Suit;
using Action = SheddingCardGames.Domain.Action;
using DealContext = SheddingCardGames.Domain.CrazyEights.DealContext;
using GameState = SheddingCardGames.Domain.CrazyEights.GameState;

namespace SheddingCardGame.Cli
{
    class Program
    {
        private static CardCollection deck;

        private static Game SetupGame(VariantName variantName)
        {
            var numberOfPlayers = 2;
            deck = new DeckBuilder().Build();
            ICrazyEightsGameBuilder gameBuilder = new CrazyEightsGameBuilder();

            var game = gameBuilder.Build(new Shuffler(), variantName, numberOfPlayers);
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());

            return game;
        }

        private static Game SetupMinimalGame(VariantName variantName)
        {
            var numberOfPlayers = 2;
            deck = new MinimalDeckBuilder(numberOfPlayers).Build();
            ICrazyEightsGameBuilder gameBuilder = new CrazyEightsGameBuilder();

            var game = gameBuilder.Build(new DummyShuffler(), variantName,  numberOfPlayers);
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());

            return game;
        }

        private static Game SetupTestGame(VariantName variantName)
        {
            var numberOfPlayers = 2;
            deck = 
                new SheddingCardGames.Domain.CrazyEights.SpecificDeckBuilder(
                    Card(9, Hearts), 
                    new CardCollection(Cards(Card(10, Spades), Card(11, Spades), Card(12, Spades), Card(13, Spades))),
                    new CardCollection(Cards(Card(9, Clubs), Card(9, Diamonds), Card(9, Hearts), Card(4, Clubs), Card(5, Clubs), Card(6, Clubs), Card(7, Clubs))),
                    new CardCollection(Cards(Card(1, Diamonds), Card(2, Diamonds), Card(3, Diamonds), Card(4, Diamonds), Card(5, Diamonds), Card(6, Diamonds), Card(7, Diamonds))))
                    .Build();
            ICrazyEightsGameBuilder gameBuilder = new CrazyEightsGameBuilder();

            var game = gameBuilder.Build(new DummyShuffler(), variantName, numberOfPlayers);
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());

            return game;
        }

        private static WhistClassic SetupWhistClassicTestGame()
        {
            var players = new List<Player> {new Player(1, "Alice"), new Player(2, "Bob"), new Player(3, "Carol"), new Player(4, "Dan") };
            var config = new WhistConfiguration();

            var deck =
                new SheddingCardGames.Domain.Whist.SpecificDeckBuilder(
                        new CardCollection(Cards(Card(1, Clubs), Card(2, Clubs), Card(3, Clubs), Card(4, Clubs), Card(5, Clubs), Card(6, Clubs), Card(7, Clubs), Card(8, Clubs), Card(9, Clubs), Card(10, Clubs), Card(11, Clubs), Card(12, Clubs), Card(13, Clubs))),
                        new CardCollection(Cards(Card(1, Diamonds), Card(2, Diamonds), Card(3, Diamonds), Card(4, Diamonds), Card(5, Diamonds), Card(6, Diamonds), Card(7, Diamonds), Card(8, Diamonds), Card(9, Diamonds), Card(10, Diamonds), Card(11, Diamonds), Card(12, Diamonds), Card(13, Diamonds))),
                        new CardCollection(Cards(Card(1, Hearts), Card(2, Hearts), Card(3, Hearts), Card(4, Hearts), Card(5, Hearts), Card(6, Hearts), Card(7, Hearts), Card(8, Hearts), Card(9, Hearts), Card(10, Hearts), Card(11, Hearts), Card(12, Hearts), Card(13, Hearts))),
                        new CardCollection(Cards(Card(1, Spades), Card(2, Spades), Card(3, Spades), Card(4, Spades), Card(5, Spades), Card(6, Spades), Card(7, Spades), Card(8, Spades), Card(9, Spades), Card(10, Spades), Card(11, Spades), Card(12, Spades), Card(13, Spades)))
                    ).Build();

            return new WhistClassic(players.ToArray(), config, new DummyShuffler(), new DummyPlayerChooser(), deck);
        }

        static void Main(string[] args)
        {
            //PlayCrazyEightsOlsenOlsen();
            new PlayWhistClassic().Play(SetupWhistClassicTestGame());
        }


        private static void PlayCrazyEightsOlsenOlsen()
        {
            Console.WriteLine("Playing CrazyEights Basic Variant");
            
            var variantNameInput = "OlsenOlsen";
            //var variantNameInput = "Basic";

            VariantName variantName = Enum.Parse<VariantName>(variantNameInput);
            var game = SetupTestGame(variantName);

            foreach (var player in game.Players.Values)
                Console.WriteLine($"{player.Number}. {player.Name}");

            Console.WriteLine("----------");

            game.Deal(new DealContext(deck));

            StateOfTurn currentTurn = null;
            var currentStateOfPlay = game.GameState.CurrentStateOfPlay;

            while (currentTurn == null || !currentStateOfPlay.HasWinner)
            {
                currentTurn = game.GameState.CurrentStateOfTurn;
                RenderTurn(currentTurn, game.GameState);
                if (currentTurn.CurrentAction == Action.SelectSuit)
                    SelectSuit(game, currentTurn);
                else
                    Play(game, currentTurn);
            }

            Console.WriteLine($"{currentStateOfPlay.Winner.Name} has won!");
        }

        private static void SelectSuit(Game game, StateOfTurn currentTurn)
        {
            Console.WriteLine($"Select a Suit:");
            var selectedSuitInput = Console.ReadLine();
            selectedSuitInput = selectedSuitInput.Trim().ToUpperInvariant();

            var selectedSuit = new SuitParser().Parse(selectedSuitInput);

            Console.WriteLine($"{currentTurn.PlayerToPlay.Name} selects Suit: {selectedSuit}");

            game.SelectSuit(new SelectSuitContext(currentTurn.PlayerToPlay, selectedSuit));
        }

        private static IEnumerable<Card> ParseInput(string input)
        {
            var cardsString = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

            var cards = new List<Card>();

            foreach (var cardString in cardsString)
            {
                var rankString = cardString.Count() > 2 ? cardString.Substring(0, 2) : cardString.Substring(0, 1);
                var suitString = cardString.Last().ToString().Trim().ToUpperInvariant();
                var rank = int.Parse(rankString);
                var suit = new SuitParser().Parse(suitString);
                
                cards.Add(Card(rank, suit));
            }

            return cards;
        }

        private static void Play(Game game, StateOfTurn currentTurn)
        {
            if (currentTurn.CurrentAction == Action.Take)
            {
                Console.WriteLine($"No valid plays, press any key to Take a card");
                Console.ReadKey();
                game.Take(new TakeContext(currentTurn.PlayerToPlay));
                Console.WriteLine($"Taken: {game.GameState.CurrentStateOfTurn.TakenCard}");
                return;
            }
            
            Console.WriteLine($"Enter {currentTurn.PlayerToPlay.Name}'s play in format RS (RankSuit):");
            var playInput = Console.ReadLine();

            var parsedInput = ParseInput(playInput).ToList();
            Console.WriteLine($"{currentTurn.PlayerToPlay.Name} plays: {string.Join(" ", parsedInput)}");

            var playResult = game.Play(new SheddingCardGames.Domain.CrazyEights.PlayContext(currentTurn.PlayerToPlay, Cards(parsedInput.ToArray())));
            Console.WriteLine($"IsValidPlay: {playResult}");
        }

        private static void RenderTurn(StateOfTurn turn, GameState gameState)
        {
            var currentTable = gameState.CurrentTable;

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Turn {turn.TurnNumber}");
            Console.WriteLine($"PlayerToPlay: {turn.PlayerToPlay.Name}");
            Console.WriteLine($"NextAction: {turn.CurrentAction}");
            Console.WriteLine($"SelectedSuit: {gameState.CurrentStateOfTurn?.SelectedSuit}");
            Console.WriteLine($"Stock pile: {currentTable.StockPile.Cards.Count()} cards");
            Console.WriteLine($"Discard pile: {currentTable.DiscardPile.CardToMatch} ({currentTable.DiscardPile.RestOfCards.Count()} other cards)");
            
            foreach (var player in currentTable.Players)
            {
                var playerHand = string.Join(", ", player.Hand.Cards.Select(x => x.ToString()));
                Console.WriteLine($"Player {player.Number} ({player.Name}) hand: {playerHand} ");
            }
            
            Console.WriteLine("--------------------------------------------------");
        }
    }
}

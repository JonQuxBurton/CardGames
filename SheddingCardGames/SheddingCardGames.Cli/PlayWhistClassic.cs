using System;
using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.Domain.Whist;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CardsUtils;

namespace SheddingCardGame.Cli
{
    public class PlayWhistClassic
    {
        public void Play(WhistClassic game)
        {
            Console.WriteLine("Playing Whist Classic Variant");
            foreach (var player in game.Players.Values)
                Console.WriteLine($"{player.Number}. {player.Name}");

            Console.WriteLine("----------");

            Console.WriteLine("ChooseStartingPlayer...");
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());
            Console.WriteLine($"Chosen StartingPlayer: {game.GameState.GameSetup.PlayerToStart.Name}");

            Console.WriteLine("Deal...");
            game.Deal(new SheddingCardGames.Domain.Whist.DealContext(game.Deck));

            foreach (var player in game.Players.Values)
                Console.WriteLine($"{player.Number}. {player.Name}: {CardsToString(player.Hand)} ");

            ExecuteGameLoop(game);
        }

        private void ExecuteGameLoop(WhistClassic game)
        {
            StateOfTrick currentTurn = null;
            var currentStateOfPlay = game.GameState.CurrentStateOfPlay;

            while (currentTurn == null || !currentStateOfPlay.HasWinner)
            {
                currentTurn = game.GameState.CurrentStateOfTrick;
                RenderTrick(currentTurn, game.GameState);
                Play(game, currentTurn);
            }

            Console.WriteLine($"{currentStateOfPlay.Winner.Name} has won!");

        }

        private static void RenderTrick(StateOfTrick trick, GameState gameState)
        {
            var currentTable = gameState.CurrentTable;

            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"Trick {trick.TrickNumber}");
            Console.WriteLine($"Cards: {CardsToString(gameState.CurrentTable.Trick.CardCollection)}");

            if (trick.HasWinner) 
                Console.WriteLine($"Won by: {trick.Winner.Name}");
            else
                Console.WriteLine($"PlayerToPlay: {trick.PlayerToPlay.Name}");

            foreach (var player in currentTable.Players)
                Console.WriteLine($"{player.Number}. {player.Name}: {CardsToString(player.Hand)} ");

            Console.WriteLine("--------------------------------------------------");
        }

        private static void Play(WhistClassic game, StateOfTrick currentTrick)
        {
            Console.WriteLine($"Enter {currentTrick.PlayerToPlay.Name}'s play in format RS (RankSuit):");
            var playInput = Console.ReadLine();

            var parsedInput = ParseInput(playInput).ToList();
            Console.WriteLine($"{currentTrick.PlayerToPlay.Name} plays: {string.Join(" ", parsedInput)}");

            var playResult = game.Play(new PlayContext(currentTrick.PlayerToPlay, parsedInput.First()));
            Console.WriteLine($"IsValidPlay: {playResult}");
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

        private static string CardsToString(CardCollection cards)
        {
            return string.Join(",", cards.Cards.Select(x => x.ToString()));
        }
    }
}
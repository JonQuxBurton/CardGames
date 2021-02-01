using System.Linq;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class GameBuilder
    {
        private CardCollection discardPileCards = new CardCollection();
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private readonly IShuffler shuffler = new DummyShuffler();
        private int startingPlayerNumber = 1;
        private CardCollection stockPile = new CardCollection();

        public GameBuilder WithPlayer1Hand(CardCollection hand)
        {
            player1Hand = hand;
            return this;
        }

        public GameBuilder WithPlayer2Hand(CardCollection hand)
        {
            player2Hand = hand;
            return this;
        }

        public GameBuilder WithDiscardCard(Card card)
        {
            discardPileCards = new CardCollection(card);
            return this;
        }

        public GameBuilder WithDiscardPile(CardCollection cards)
        {
            discardPileCards = cards;
            return this;
        }

        public GameBuilder WithStockPile(CardCollection cards)
        {
            stockPile = cards;
            return this;
        }

        public GameBuilder WithStartingPlayer(int playerNumber)
        {
            startingPlayerNumber = playerNumber;
            return this;
        }

        public Game Build()
        {
            var player1 = new Player(1);
            var player2 = new Player(2);

            var deck = new SpecificDeckBuilder(player1Hand, player2Hand, discardPileCards.Cards.First(), stockPile).Build();
            var rules = new Rules(player1Hand.Cards.Count());
            var game = new Game(rules, shuffler, new Dealer(rules, shuffler, deck), new[] {player1, player2});

            game.ChooseStartingPlayer(startingPlayerNumber);
            game.Deal();

            return game;
        }
    }
}
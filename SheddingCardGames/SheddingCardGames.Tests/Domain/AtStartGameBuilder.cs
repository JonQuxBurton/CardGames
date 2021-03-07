using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Tests.Domain
{
    public class AtStartGameBuilder
    {
        private readonly IShuffler shuffler = new DummyShuffler();
        private Card discardCard = new Card(1, Suit.Clubs);
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private int startingPlayerNumber = 1;
        private CardCollection stockPile = new CardCollection();

        public AtStartGameBuilder WithPlayer1Hand(CardCollection hand)
        {
            player1Hand = hand;
            return this;
        }

        public AtStartGameBuilder WithPlayer2Hand(CardCollection hand)
        {
            player2Hand = hand;
            return this;
        }

        public AtStartGameBuilder WithDiscardCard(Card card)
        {
            discardCard = card;
            return this;
        }

        public AtStartGameBuilder WithStockPile(CardCollection cards)
        {
            stockPile = cards;
            return this;
        }

        public AtStartGameBuilder WithStartingPlayer(int playerNumber)
        {
            startingPlayerNumber = playerNumber;
            return this;
        }

        public Game Build()
        {
            var player1 = new Player(1, "Alice");
            var player2 = new Player(2, "Bob");

            var deck = new SpecificDeckBuilder(player1Hand, player2Hand, discardCard, stockPile).Build();
            var rules = new Rules(player1Hand.Cards.Count());
            var game = new Game(rules, shuffler, new Dealer(rules), new[] {player1, player2}, deck);

            game.ChooseStartingPlayer(startingPlayerNumber);
            game.Deal();

            return game;
        }
    }
}
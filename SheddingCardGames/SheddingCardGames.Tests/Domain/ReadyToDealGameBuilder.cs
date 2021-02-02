using System.Linq;
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class ReadyToDealGameBuilder
    {
        private readonly IShuffler shuffler = new DummyShuffler();
        private Card discardCard = new Card(1, Suit.Clubs);
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection stockPile = new CardCollection();
        private int startingPlayerNumber = 1;

        public ReadyToDealGameBuilder WithPlayer1Hand(CardCollection hand)
        {
            player1Hand = hand;
            return this;
        }

        public ReadyToDealGameBuilder WithPlayer2Hand(CardCollection hand)
        {
            player2Hand = hand;
            return this;
        }

        public ReadyToDealGameBuilder WithDiscardCard(Card card)
        {
            discardCard = card;
            return this;
        }

        public ReadyToDealGameBuilder WithStockPile(CardCollection cards)
        {
            stockPile = cards;
            return this;
        }

        public ReadyToDealGameBuilder WithStartingPlayer(int playerNumber)
        {
            startingPlayerNumber = playerNumber;
            return this;
        }

        public Game Build()
        {
            var player1 = new Player(1);
            var player2 = new Player(2);

            var deck = new SpecificDeckBuilder(player1Hand, player2Hand, discardCard, stockPile).Build();
            var rules = new Rules(player1Hand.Cards.Count());
            var game = new Game(rules, shuffler, new Dealer(rules, shuffler, deck), new[] { player1, player2 });
            game.ChooseStartingPlayer(startingPlayerNumber);

            return game;
        }
    }
}
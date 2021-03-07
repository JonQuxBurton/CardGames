using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Tests.Domain
{
    public class ReadyToDealGameBuilder
    {
        private IShuffler shuffler = new DummyShuffler();
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

        public ReadyToDealGameBuilder WithShuffler(IShuffler withShuffler)
        {
            shuffler = withShuffler;
            return this;
        }
        
        public Game Build()
        {
            var sampleData = new SampleData();
            var player1 = sampleData.Player1;
            player1.Hand = player1Hand;
            var player2 = sampleData.Player2;
            player2.Hand = player2Hand;

            var deck = new SpecificDeckBuilder(player1Hand, player2Hand, discardCard, stockPile).Build();
            var rules = new Rules(player1Hand.Cards.Count());
            var game = new Game(rules, shuffler, new Dealer(rules), new[] { player1, player2 }, deck);
            game.ChooseStartingPlayer(startingPlayerNumber);

            return game;
        }
    }
}
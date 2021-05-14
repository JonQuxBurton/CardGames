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
        private CardCollection player3Hand = new CardCollection();
        private CardCollection stockPile = new CardCollection();
        private int startingPlayerNumber = 1;
        private int numberOfPlayers = 2;

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
        public ReadyToDealGameBuilder WithPlayer3Hand(CardCollection hand)
        {
            numberOfPlayers = 3;
            player3Hand = hand;
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

        public ReadyToDealGameBuilder WithStartingPlayer(int withPlayerNumber)
        {
            startingPlayerNumber = withPlayerNumber;
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

            var rules = new Rules(player1Hand.Cards.Count());

            CardCollection deck;
            Game game;

            if (numberOfPlayers > 2)
            {
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;
                deck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2, player3 });
            }
            else
            {
                deck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2 });
            }
            
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(sampleData.GetPlayer(startingPlayerNumber)));

            return game;
        }
    }
}
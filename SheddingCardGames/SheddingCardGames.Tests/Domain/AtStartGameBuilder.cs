using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Tests.Domain
{
    public class AtStartGameBuilder
    {
        private readonly IShuffler shuffler = new DummyShuffler();
        private Card discardCard = new Card(1, Suit.Clubs);
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection player3Hand = new CardCollection();
        private int startingPlayerNumber = 1;
        private CardCollection stockPile = new CardCollection();
        private int numberOfPlayers = 2;

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
        
        public AtStartGameBuilder WithPlayer3Hand(CardCollection hand)
        {
            numberOfPlayers = 3;
            player3Hand = hand;
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

        public AtStartGameBuilder WithStartingPlayer(int withPlayerNumber)
        {
            startingPlayerNumber = withPlayerNumber;
            return this;
        }

        public Game Build()
        {
            var sampleData = new SampleData();
            var player1 = sampleData.Player1;
            var player2 = sampleData.Player2;

            CardCollection deck;
            Game game;

            if (numberOfPlayers > 2)
            {
                var rules = new CrazyEightsRules(NumberOfPlayers.Three);
                var player3 = sampleData.Player3;
                deck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2, player3 });
            }
            else
            {
                var rules = new CrazyEightsRules(NumberOfPlayers.Two);
                deck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2 });
            }
            
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(sampleData.GetPlayer(startingPlayerNumber)));
            game.Deal();

            return game;
        }
    }
}
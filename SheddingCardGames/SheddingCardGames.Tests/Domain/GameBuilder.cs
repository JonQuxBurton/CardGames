using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using static SheddingCardGames.Domain.CrazyEightsRules;

namespace SheddingCardGames.Tests.Domain
{
    public class GameBuilder
    {
        private IShuffler shuffler = new DummyShuffler();
        private Card discardCard = new Card(1, Suit.Clubs);
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection player3Hand = new CardCollection();
        private CardCollection stockPile = new CardCollection();
        private int startingPlayerNumber = 1;
        private int numberOfPlayers = 2;
        private SampleData sampleData;

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
        public GameBuilder WithPlayer3Hand(CardCollection hand)
        {
            numberOfPlayers = 3;
            player3Hand = hand;
            return this;
        }

        public GameBuilder WithDiscardCard(Card card)
        {
            discardCard = card;
            return this;
        }

        public GameBuilder WithStockPile(CardCollection cards)
        {
            stockPile = cards;
            return this;
        }

        public GameBuilder WithStartingPlayer(int withPlayerNumber)
        {
            startingPlayerNumber = withPlayerNumber;
            return this;
        }

        public GameBuilder WithShuffler(IShuffler withShuffler)
        {
            shuffler = withShuffler;
            return this;
        }

        private Game Build()
        {
            var player1 = sampleData.Player1;
            var player2 = sampleData.Player2;

            IRules rules;
            CardCollection deck;
            Game game;

            if (numberOfPlayers > 2)
            {
                rules = new CrazyEightsRules(NumberOfPlayers.Three);
                var player3 = sampleData.Player3;
                deck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand, player3Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2, player3 });
            }
            else
            {
                rules = new CrazyEightsRules(NumberOfPlayers.Two);
                deck = new SpecificDeckBuilder(discardCard, stockPile, player1Hand, player2Hand).Build();
                game = new Game(new Variant(VariantName.OlsenOlsen, new OlsenOlsenVariantCommandFactory(rules, shuffler)), deck, new[] { player1, player2 });
            }

            return game;
        }

        public Game BuildReadyToDealGame()
        {
            sampleData = new SampleData();
            var player1 = sampleData.Player1;
            player1.Hand = player1Hand;
            var player2 = sampleData.Player2;
            player2.Hand = player2Hand;

            if (numberOfPlayers > 2)
            {
                var player3 = sampleData.Player3;
                player3.Hand = player3Hand;
            }

            var game = Build();
            
            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(sampleData.GetPlayer(startingPlayerNumber)));

            return game;
        }

        public Game BuildAtStartGame()
        {
            sampleData = new SampleData();

            var game = Build();

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(sampleData.GetPlayer(startingPlayerNumber)));
            game.Deal();

            return game;
        }
    }
}
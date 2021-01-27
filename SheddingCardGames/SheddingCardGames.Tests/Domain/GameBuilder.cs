using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class GameBuilder
    {
        private CardCollection discardPileCards = new CardCollection();
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private IShuffler shuffler = new DummyShuffler();
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

        public GameBuilder WithShuffler(IShuffler shuffler)
        {
            this.shuffler = shuffler;
            return this;
        }

        public Game Build()
        {
            var player1 = new Player(1) {Hand = player1Hand};
            var player2 = new Player(2) {Hand = player2Hand};
            var discardPile = new DiscardPile(discardPileCards.Cards);
            discardPile.TurnUpTopCard();

            var game = new Game(new Rules(), shuffler, new[] {player1, player2});

            var board = new Board(player1, player2, stockPile, discardPile);
            game.Setup(board, startingPlayerNumber);

            return game;
        }
    }
}
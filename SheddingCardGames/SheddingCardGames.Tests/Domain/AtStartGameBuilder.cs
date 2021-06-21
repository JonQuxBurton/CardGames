using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class AtStartGameBuilder
    {
        private readonly Game game;
        private CardCollection deck;
        private DiscardPile discardPile = new DiscardPile(CardsUtils.Cards(CardsUtils.Card(1, Suit.Clubs)));
        private int numberOfPlayers = 2;
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection player3Hand = new CardCollection();
        private StockPile stockPile = new StockPile(new CardCollection());

        public AtStartGameBuilder(Game game)
        {
            this.game = game;
        }

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
            discardPile = new DiscardPile(CardsUtils.Cards(card));
            discardPile.TurnUpTopCard();
            return this;
        }

        public AtStartGameBuilder WithStockPile(CardCollection withStockPile)
        {
            stockPile = new StockPile(withStockPile);
            return this;
        }

        public Game Build()
        {
            if (numberOfPlayers > 2)
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand,
                    player2Hand, player3Hand).Build();
            else
                deck = new SpecificDeckBuilder(discardPile.AllCards, new CardCollection(stockPile.Cards), player1Hand,
                    player2Hand).Build();

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext());
            game.Deal(new DealContext(deck));

            return game;
        }
    }
}
using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class GameCreator
    {
        public GameCreator()
        {
            new CardCollectionBuilder().Build();
        }

        public Game CreateNewGame(Player player1, Player player2)
        {
            return new Game(new Rules(), new DummyShuffler(), 
                new Dealer(new Rules(), new DummyShuffler(), new CardCollection()), new[] { player1, player2 });
        }

        public Game CreateSetupGame(Card[] player1Hand, Card[] player2Hand, Card[] withDiscardPile, Card[] stockPile = null)
        {
            stockPile ??= new Card[0];

            var discardPile = new DiscardPile(withDiscardPile);
            discardPile.TurnUpTopCard();
            var player1 = new Player(1) {Hand = new CardCollection(player1Hand)};
            var player2 = new Player(2) {Hand = new CardCollection(player2Hand)};
                
            var game = new Game(new Rules(), new DummyShuffler(), new Dealer(new Rules(), new DummyShuffler(), new CardCollection()), new [] { player1, player2});
                
            var board = new Board(player1, player2, new CardCollection(stockPile), discardPile);
            game.ChooseStartingPlayer(1);
                
            return game;
        }
    }
}
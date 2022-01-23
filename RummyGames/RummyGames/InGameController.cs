using System.Linq;
using CardGamesDomain;

namespace RummyGames
{
    public class InGameController
    {
        private readonly IShuffler shuffler;
        private int numberOfCardsToDeal;

        public InGameController(IShuffler shuffler)
        {
            this.shuffler = shuffler;
            numberOfCardsToDeal = 10;
        }

        public InGameState ShuffleDeck(InGameState inGameState)
        {
            var shuffledDeck = new Deck(shuffler.Shuffle(inGameState.Table.Deck.Cards));

            return new InGameState(inGameState.GameId, new Table(inGameState.Table.Players, shuffledDeck, null),
                inGameState.StartingPlayer);
        }

        public InGameState Deal(InGameState inGameState)
        {
            var currentTable = inGameState.Table;
            var cards = currentTable.Deck.Cards.ToArray();

            Player newPlayer1 = currentTable.Players.ElementAt(0);
            Player newPlayer2 = currentTable.Players.ElementAt(1);
            var deckCounter = 0;
            
            Card nextCard;

            for (var i = 0; i < numberOfCardsToDeal; i++)
            {
                nextCard = cards.ElementAt(deckCounter++);
                newPlayer1 = MoveCardToPlayer(newPlayer1, nextCard);

                nextCard = cards.ElementAt(deckCounter++);
                newPlayer2 = MoveCardToPlayer(newPlayer2, nextCard);
            }

            nextCard = cards.ElementAt(deckCounter++);
            var discardPile = new DiscardPile(nextCard);
            var stockPile = new StockPile(cards.Skip(deckCounter));
            var newTable = new Table(new []{newPlayer1, newPlayer2}, currentTable.Deck, discardPile, stockPile);

            return new InGameState(inGameState.GameId, newTable, inGameState.StartingPlayer );
        }

        private static Player MoveCardToPlayer(Player currentPlayer, Card cardToDeal)
        {
            var newPlayer = new Player(currentPlayer.Id, currentPlayer.Name,
                new Hand(currentPlayer.Hand.Cards.Append(cardToDeal)));
            return newPlayer;
        }
    }
}
using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class Dealer : IDealer
    {
        private readonly IRules rules;
        private readonly IShuffler shuffler;
        private readonly CardCollection deck;

        public Dealer(IRules rules, IShuffler shuffler, CardCollection deck)
        {
            this.rules = rules;
            this.shuffler = shuffler;
            this.deck = deck;
        }
        
        public Board Deal(IEnumerable<Player> players)
        {
            var shuffled = shuffler.Shuffle(deck.Cards);

            var board = new Board(players.ElementAt(0), players.ElementAt(1), new StockPile(new CardCollection(shuffled)), new DiscardPile());

            for (var i = 0; i < rules.GetHandSize(); i++)
            {
                if (!board.StockPile.IsEmpty())
                {
                    board.TakeCardFromStockPile(board.Player1);
                    board.TakeCardFromStockPile(board.Player2);
                }
            }

            board.TurnUpDiscardCard();

            return board;
        }
    }
}
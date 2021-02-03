using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class Dealer : IDealer
    {
        private readonly IRules rules;

        public Dealer(IRules rules)
        {
            this.rules = rules;
        }
        
        public Board Deal(IEnumerable<Player> players, CardCollection cardsToDeal)
        {
            var board = new Board(players.ElementAt(0), players.ElementAt(1), new StockPile(cardsToDeal), new DiscardPile());

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
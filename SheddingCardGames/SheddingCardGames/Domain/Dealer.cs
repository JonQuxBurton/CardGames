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
            var playersArray = players as Player[] ?? players.ToArray();
            var board = new Board(new StockPile(cardsToDeal), new DiscardPile(), playersArray.ToArray());

            for (var i = 0; i < rules.GetHandSize(); i++)
            {
                if (board.StockPile.IsEmpty()) break;

                for (var j=0; j< playersArray.Count(); j++)
                    board.TakeCardFromStockPile(board.Players[j]);
                
            }

            board.TurnUpDiscardCard();

            return board;
        }
    }
}
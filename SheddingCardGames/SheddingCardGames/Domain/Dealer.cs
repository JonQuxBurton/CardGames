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
        
        public Board Build(IEnumerable<Player> players)
        {
            var shuffled = shuffler.Shuffle(deck.Cards);

            var board = new Board(players.ElementAt(0), players.ElementAt(1), new CardCollection(shuffled), new DiscardPile());

            board.Deal(rules.GetHandSize());
            board.TurnUpDiscardCard();

            return board;
        }
    }
}
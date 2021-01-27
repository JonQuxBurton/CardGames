using System.Collections.Generic;
using System.Linq;

namespace SheddingCardGames.Domain
{
    public class BoardBuilder
    {
        private readonly IRules rules;
        private readonly IShuffler shuffler;

        public BoardBuilder(IRules rules, IShuffler shuffler)
        {
            this.rules = rules;
            this.shuffler = shuffler;
        }
        
        public Board Build(CardCollection deck, IEnumerable<Player> players)
        {
            var shuffled = shuffler.Shuffle(deck.Cards);
            var board = new Board(players.ElementAt(0), players.ElementAt(1), new CardCollection(shuffled), new DiscardPile());

            board.Deal(rules.GetHandSize());
            board.TurnUpDiscardCard();

            return board;
        }
    }
}
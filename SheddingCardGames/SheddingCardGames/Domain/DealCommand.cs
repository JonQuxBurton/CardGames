using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class DealCommand : GameCommand
    {
        private readonly GameState currentGameState;
        private readonly IDealer dealer;
        private readonly IRules rules;
        private readonly CardCollection deck;
        private readonly Player[] players;

        private readonly IShuffler shuffler;

        public DealCommand(IShuffler shuffler, IDealer dealer, IRules rules, GameState currentGameState, CardCollection deck,
            Player[] players)
        {
            this.shuffler = shuffler;
            this.dealer = dealer;
            this.rules = rules;
            this.currentGameState = currentGameState;
            this.deck = deck;
            this.players = players;
        }

        public override ActionResult IsValid()
        {
            return new ActionResult(true, ActionResultMessageKey.Success);
        }

        public override GameState Execute()
        {
            var shuffled = shuffler.Shuffle(deck);

            var table = dealer.Deal(players, shuffled, currentGameState.Events);
            currentGameState.CurrentTable = table;
            currentGameState.Events.Add(new DealCompleted(GetNextEventNumber(currentGameState.Events)));

            AddFirstTurn(currentGameState.CurrentPlayer);

            return currentGameState;
        }

        private void AddFirstTurn(Player nextPlayer)
        {
            var validPlays = GetValidPlays(nextPlayer.Hand, currentGameState.CurrentTable.DiscardPile.CardToMatch, 1, null)
                .ToArray();

            var newTurn = new CurrentTurn(1,
                nextPlayer,
                validPlays,
                false,
                null,
                GetNextAction(validPlays), null);
            currentGameState.TurnNumber = 1;
            currentGameState.CurrentTurn = newTurn;
        }

        private IEnumerable<Card> GetValidPlays(CardCollection hand, Card discard, int turnNumber, Suit? selectedSuit)
        {
            return rules.GetValidPlays(discard, hand, turnNumber, selectedSuit);
        }

        private Action GetNextAction(IEnumerable<Card> validPlays)
        {
            return !validPlays.Any() ? Action.Take : Action.Play;
        }
    }
}
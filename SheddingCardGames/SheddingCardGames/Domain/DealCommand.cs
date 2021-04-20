using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain.Events;
using SheddingCardGames.UiLogic;

namespace SheddingCardGames.Domain
{
    public class DealCommand : GameCommand
    {
        private readonly GameState currentGameState;
        private readonly IRules rules;
        private readonly CardCollection deck;
        private readonly Player[] players;

        private readonly IShuffler shuffler;

        public DealCommand(IShuffler shuffler, IRules rules, GameState currentGameState, CardCollection deck,
            Player[] players)
        {
            this.shuffler = shuffler;
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

            var table = Deal(shuffled, currentGameState.Events);
            currentGameState.CurrentTable = table;
            currentGameState.Events.Add(new DealCompleted(GetNextEventNumber(currentGameState.Events)));

            AddFirstTurn(currentGameState.CurrentPlayer);

            return currentGameState;
        }

        private Table Deal(CardCollection cardsToDeal, List<DomainEvent> events)
        {
            var playersArray = players as Player[] ?? players.ToArray();
            var table = new Table(new StockPile(cardsToDeal), new DiscardPile(), playersArray.ToArray());

            for (var i = 0; i < rules.GetHandSize(); i++)
            {
                if (table.StockPile.IsEmpty()) break;

                for (var j = 0; j < playersArray.Count(); j++)
                {
                    var player = table.Players[j];
                    var takenCard = table.MoveCardFromStockPileToPlayer(player);
                    events.Add(new CardMoved(events.Select(x => x.Number).DefaultIfEmpty().Max() + 1, takenCard,
                        CardMoveSources.StockPile, GetPlayerSource(player)));
                }
            }

            var cardTurnedUp = table.MoveCardFromStockPileToDiscardPile();
            events.Add(new CardMoved(events.Select(x => x.Number).DefaultIfEmpty().Max() + 1, cardTurnedUp,
                CardMoveSources.StockPile, CardMoveSources.DiscardPile));

            return table;
        }

        private static string GetPlayerSource(Player player)
        {
            return CardMoveSources.PlayerHand(player.Number);
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
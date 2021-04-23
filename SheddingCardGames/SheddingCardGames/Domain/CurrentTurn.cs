using System.Collections.Generic;

namespace SheddingCardGames.Domain
{
    public class CurrentTurn
    {
        public CurrentTurn(int turnNumber,
            Player playerToPlay,
            IEnumerable<Card> validPlays,
            Action nextAction)
        {
            TurnNumber = turnNumber;
            PlayerToPlay = playerToPlay;
            ValidPlays = validPlays;
            NextAction = nextAction;
        }

        public int TurnNumber { get; }
        public Player PlayerToPlay { get; }
        public IEnumerable<Card> ValidPlays { get; }
        public Action NextAction { get; }
    }
}
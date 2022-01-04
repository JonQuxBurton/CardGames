using System.Collections.Immutable;
using System.Linq;

namespace SheddingCardGames.Domain.Whist
{
    public class Play
    {
        public Play(Player player, Card card)
        {
            Player = player;
            Card = card;
        }
        public Player Player { get; }
        public Card Card { get;  }
    }

    public class StateOfTrick
    {
        public StateOfTrick(int trickNumber, 
            Player playerToStart,
            Player playerToPlay,
            ImmutableList<Play> plays = null,
            Player winner = null)
        {
            TrickNumber = trickNumber;
            PlayerToStart = playerToStart;
            PlayerToPlay = playerToPlay;
            Plays = plays;
            Winner = winner;
        }

        public Suit? TrickSuit
        {
            get
            {
                if (Plays?.FirstOrDefault() == null)
                    return null;

                return Plays.First().Card.Suit;
            }
        }

        public int TrickNumber { get; }
        public Player PlayerToStart { get; }
        public Player PlayerToPlay { get; }
        public ImmutableList<Play> Plays { get; }
        public Player Winner { get; }
        public bool HasWinner => Winner != null;
    }
}
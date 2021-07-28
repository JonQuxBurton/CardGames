using System.Collections.Immutable;
using System.Linq;

namespace SheddingCardGames.Domain.Whist
{
    public class StateOfTrick
    {
        public StateOfTrick(int trickNumber, 
            Player playerToStart,
            Player playerToPlay,
            ImmutableList<Card> cardsPlayed = null)
        {
            TrickNumber = trickNumber;
            PlayerToStart = playerToStart;
            PlayerToPlay = playerToPlay;
            CardsPlayed = cardsPlayed;
        }

        public Suit? TrickSuit
        {
            get
            {
                if (CardsPlayed == null || CardsPlayed.FirstOrDefault() == null)
                    return null;

                return CardsPlayed.First().Suit;
            }
        }

        public int TrickNumber { get; }
        public Player PlayerToStart { get; }
        public Player PlayerToPlay { get; }
        public ImmutableList<Card> CardsPlayed { get; }
    }
}
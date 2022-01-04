using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain.Whist
{
    public class TrickCompleted : DomainEvent
    {
        public int WinnerPlayerNumber { get; }
        
        public TrickCompleted(int number, int winnerPlayerNumber) : base(number)
        {
            WinnerPlayerNumber = winnerPlayerNumber;
        }
    }
}
using SheddingCardGames.Domain.Events;

namespace SheddingCardGames.Domain.Whist
{
    public class TrickStarted : DomainEvent
    {
        public TrickStarted(int number) : base(number)
        {
        }
    }
}
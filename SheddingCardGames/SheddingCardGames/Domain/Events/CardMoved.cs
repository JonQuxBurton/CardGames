namespace SheddingCardGames.Domain.Events
{
    public class CardMoved : DomainEvent
    {
        public Card Card { get; }
        public string FromSource { get; }
        public string ToSource { get; }

        public CardMoved(int number, Card card, string fromSource, string toSource): base(number)
        {
            Card = card;
            FromSource = fromSource;
            ToSource = toSource;
        }
    }
}
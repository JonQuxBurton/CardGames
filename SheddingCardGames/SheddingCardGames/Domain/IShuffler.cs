namespace SheddingCardGames.Domain
{
    public interface IShuffler
    {
        CardCollection Shuffle(CardCollection cards);
    }
}
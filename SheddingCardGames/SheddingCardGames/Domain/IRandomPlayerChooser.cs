using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public interface IRandomPlayerChooser
    {
        Player ChoosePlayer(IImmutableList<Player> players);
    }
}
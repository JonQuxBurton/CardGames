using System.Collections.Immutable;
using System.Linq;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class DummyPlayerChooser : IRandomPlayerChooser
    {
        private readonly Player chosenPlayer;

        public DummyPlayerChooser()
        {
        }

        public DummyPlayerChooser(Player chosenPlayer)
        {
            this.chosenPlayer = chosenPlayer;
        }

        public Player ChoosePlayer(IImmutableList<Player> players)
        {
            if (chosenPlayer == null)
                return players.First();

            return chosenPlayer;
        }
    }
}
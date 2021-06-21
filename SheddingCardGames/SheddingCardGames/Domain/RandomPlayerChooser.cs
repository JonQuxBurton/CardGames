using System;
using System.Collections.Immutable;

namespace SheddingCardGames.Domain
{
    public class RandomPlayerChooser : IRandomPlayerChooser
    {
        private readonly Random random;

        public RandomPlayerChooser()
        {
            random = new Random();
        }

        public Player ChoosePlayer(IImmutableList<Player> players)
        {
            return players[random.Next(players.Count)];
        }
    }
}
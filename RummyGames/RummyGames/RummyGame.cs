using System;

namespace RummyGames
{
    public class RummyGame
    {
        public Guid Id { get; }
        public Player Host { get; }
        public Player Guest { get; }

        private RummyGame(Guid id, Player host, Player guest=null)
        {
            Id = id;
            Host = host;
            Guest = guest;
        }

        public static RummyGame Create(Guid id, Player host)
        {
            return new RummyGame(id, host);
        }

        public static RummyGame WithJoinPlayer(RummyGame currentGame, Player playerToJoin)
        {
            return new RummyGame(currentGame.Id, currentGame.Host, playerToJoin);
        }
    }
}
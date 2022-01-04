namespace SheddingCardGames.Domain.Whist
{
    public class CurrentTrickBuilder
    {
        public StateOfTrick BuildFirstTrick(GameState gameState, Player nextPlayer)
        {
            return new StateOfTrick(1, nextPlayer, nextPlayer);
        }

        public StateOfTrick AddCard(GameState gameState, Card playContextCardPlayed, Player nextPlayer)
        {
            return new StateOfTrick(1, nextPlayer, nextPlayer);
        }
        
        public StateOfTrick AddWinner(GameState gameState, Card playContextCardPlayed, Player nextPlayer, Player winner)
        {
            return new StateOfTrick(1, nextPlayer, nextPlayer, null, winner);
        }
    }
}
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
    }
}
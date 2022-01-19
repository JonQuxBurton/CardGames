using CardGamesDomain;

namespace RummyGames
{
    public class InGameController
    {
        private readonly IShuffler shuffler;

        public InGameController(IShuffler shuffler)
        {
            this.shuffler = shuffler;
        }

        public InGameState ShuffleDeck(InGameState inGameState)
        {
            var shuffledDeck = new Deck(shuffler.Shuffle(inGameState.Table.Deck.Cards));

            return new InGameState(inGameState.GameId, new Table(inGameState.Table.Players, shuffledDeck),
                inGameState.StartingPlayer);
        }
    }
}
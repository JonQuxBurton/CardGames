using SheddingCardGames.Domain;

namespace SheddingCardGames.Tests.Domain
{
    public class ReadyToDealGameBuilder
    {
        private readonly Game game;
        private DiscardPile discardPile = new DiscardPile(CardsUtils.Cards(CardsUtils.Card(1, Suit.Clubs)));
        private int numberOfPlayers = 2;
        private CardCollection player1Hand = new CardCollection();
        private CardCollection player2Hand = new CardCollection();
        private CardCollection player3Hand = new CardCollection();
        private int startingPlayerNumber = 1;

        public ReadyToDealGameBuilder(Game game)
        {
            this.game = game;
        }

        public ReadyToDealGameBuilder WithPlayer1Hand(CardCollection hand)
        {
            player1Hand = hand;
            return this;
        }

        public ReadyToDealGameBuilder WithPlayer2Hand(CardCollection hand)
        {
            player2Hand = hand;
            return this;
        }

        public ReadyToDealGameBuilder WithPlayer3Hand(CardCollection hand)
        {
            numberOfPlayers = 3;
            player3Hand = hand;
            return this;
        }

        public ReadyToDealGameBuilder WithDiscardCard(Card card)
        {
            discardPile = new DiscardPile(CardsUtils.Cards(card));
            discardPile.TurnUpTopCard();
            return this;
        }

        public ReadyToDealGameBuilder WithStartingPlayer(int withPlayerNumber)
        {
            startingPlayerNumber = withPlayerNumber;
            return this;
        }

        public Game Build()
        {
            game.GetPlayer(1).Hand = player1Hand;
            game.GetPlayer(2).Hand = player2Hand;

            if (numberOfPlayers > 2)
                game.GetPlayer(3).Hand = player3Hand;

            game.ChooseStartingPlayer(new ChooseStartingPlayerContext(game.GetPlayer(startingPlayerNumber)));

            return game;
        }
    }
}
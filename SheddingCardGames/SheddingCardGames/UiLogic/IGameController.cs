using SheddingCardGame.UI;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public interface IGameController
    {
        void Deal();
        void SelectSuit(Suit suit);
        bool Play(Card card);
        ActionResultWithCard Take();
        GameState GameState { get; }
        Turn CurrentTurn { get;  }
    }
}
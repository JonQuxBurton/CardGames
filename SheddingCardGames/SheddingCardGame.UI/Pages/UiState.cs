using System.Collections.Generic;
using SheddingCardGames;

namespace SheddingCardGame.UI.Pages
{
    public class UiState
    {
        public IGameObject DealButton { get; set; }
        public IGameObject TakeButton { get; set; }
        public IGameObject ClubsButton { get; set; }
        public IGameObject DiamondsButton { get; set; }
        public IGameObject HeartsButton { get; set; }
        public IGameObject SpadesButton { get; set; }

        public readonly List<IGameObject> GameObjects = new List<IGameObject>();
        public readonly Dictionary<Card, CardComponent> CardGameObjects = new Dictionary<Card, CardComponent>();
        public LabelComponent TurnLabel { get; set; }
        public LabelComponent PlayerToPlayLabel { get; set; }
        public LabelComponent InvalidPlayLabel { get; set; }
        public LabelComponent SelectedSuitLabel { get; set; }

        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}

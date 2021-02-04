using System.Collections.Generic;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;

namespace SheddingCardGame.UI
{
    public class UiState
    {
        private readonly GameState gameState;

        public UiState(GameState gameState)
        {
            this.gameState = gameState;
        }

        public Board CurrentBoard => gameState.CurrentBoard;
        
        public readonly Dictionary<Card, CardComponent> CardGameObjects = new Dictionary<Card, CardComponent>();

        public readonly List<IGameObject> GameObjects = new List<IGameObject>();
        public IGameObject DealButton { get; set; }
        public IGameObject TakeButton { get; set; }
        public IGameObject ClubsButton { get; set; }
        public IGameObject DiamondsButton { get; set; }
        public IGameObject HeartsButton { get; set; }
        public IGameObject SpadesButton { get; set; }
        public LabelComponent TurnLabel { get; set; }
        public LabelComponent PlayerToPlayLabel { get; set; }
        public LabelComponent InvalidPlayLabel { get; set; }
        public LabelComponent SelectedSuitLabel { get; set; }

        public GamePhase CurrentGamePhase = GamePhase.New;
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
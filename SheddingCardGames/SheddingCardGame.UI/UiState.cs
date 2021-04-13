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

        public Table CurrentTable => gameState.CurrentTable;
        
        public readonly Dictionary<Card, CardComponent> CardGameObjects = new Dictionary<Card, CardComponent>();
        public readonly List<IGameObject> GameObjects = new List<IGameObject>();
        
        public GamePhase CurrentGamePhase = GamePhase.New;
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using SheddingCardGames.Domain;
using SheddingCardGames.UiLogic;
using Action = SheddingCardGames.Domain.Action;

namespace SheddingCardGame.UI
{
    public class BlazorGameController
    {
        private readonly ActionResultMessageMapper actionResultMessageMapper;

        public readonly Dictionary<ButtonNames, ButtonComponent> Buttons =
            new Dictionary<ButtonNames, ButtonComponent>();

        private readonly CardCollection deck;
        private readonly Game game;
        private readonly InGameUiBuilder inGameUiBuilder;
        public readonly Dictionary<LabelNames, LabelComponent> Labels = new Dictionary<LabelNames, LabelComponent>();

        private readonly List<CardComponent> selectedCards = new List<CardComponent>();

        private bool isProcessing = false;

        public BlazorGameController(InGameUiBuilder inGameUiBuilder, Game game, CardCollection deck,
            ActionResultMessageMapper actionResultMessageMapper)
        {
            this.inGameUiBuilder = inGameUiBuilder;
            this.game = game;
            this.deck = deck;
            this.actionResultMessageMapper = actionResultMessageMapper;
        }

        public CurrentTurn CurrentTurn => game.GameState.CurrentTurn;
        public CardCollection AllCards => game.GameState.CurrentTable.AllCards;

        public UiState UiState { get; set; }

        public async void Deal()
        {
            game.Deal(new DealContext(deck));
            UiState = await inGameUiBuilder.Build(this, game.GameState);

            Labels.Add(LabelNames.Turn,
                UiState.GameObjects.First(x => x.Tag == LabelNames.Turn.ToString()) as LabelComponent);
            Labels.Add(LabelNames.PlayerToPlay,
                UiState.GameObjects.First(x => x.Tag == LabelNames.PlayerToPlay.ToString()) as LabelComponent);
            Labels.Add(LabelNames.InvalidPlay,
                UiState.GameObjects.First(x => x.Tag == LabelNames.InvalidPlay.ToString()) as LabelComponent);
            Labels.Add(LabelNames.SelectedSuit,
                UiState.GameObjects.First(x => x.Tag == LabelNames.SelectedSuit.ToString()) as LabelComponent);

            Buttons.Add(ButtonNames.Take,
                UiState.GameObjects.First(x => x.Tag == ButtonNames.Take.ToString()) as ButtonComponent);

            Buttons.Add(ButtonNames.Clubs,
                UiState.GameObjects.First(x => x.Tag == ButtonNames.Clubs.ToString()) as ButtonComponent);
            Buttons.Add(ButtonNames.Diamonds,
                UiState.GameObjects.First(x => x.Tag == ButtonNames.Diamonds.ToString()) as ButtonComponent);
            Buttons.Add(ButtonNames.Hearts,
                UiState.GameObjects.First(x => x.Tag == ButtonNames.Hearts.ToString()) as ButtonComponent);
            Buttons.Add(ButtonNames.Spades,
                UiState.GameObjects.First(x => x.Tag == ButtonNames.Spades.ToString()) as ButtonComponent);
        }

        private void LogGameState()
        {
            Console.WriteLine($"CardToMatch: {game.GameState.CurrentTable.DiscardPile.CardToMatch}");
            var player1Hand = string.Join("|",
                game.GameState.CurrentTable.Players[0].Hand.Cards.Select(x => x.ToString()));
            Console.WriteLine($"Player1 Hand: {player1Hand}");
            var player2Hand = string.Join("|",
                game.GameState.CurrentTable.Players[1].Hand.Cards.Select(x => x.ToString()));
            Console.WriteLine($"Player2 Hand: {player2Hand}");
        }

        public bool Play(CardComponent cardComponent)
        {
            if (!cardComponent.IsTurnedUp)
                return false;

            if (isProcessing)
                return false;
            
            isProcessing = true;

            var cardsToPlay = new List<CardComponent>();

            if (selectedCards.Any())
                cardsToPlay.AddRange(selectedCards);
            else
                cardsToPlay.Add(cardComponent);

            var actionResult = game.Play(new PlayContext(CurrentTurn.PlayerToPlay,
                CardsUtils.Cards(cardsToPlay.Select(x => x.Card).ToArray())));
            
            if (actionResult.IsSuccess)
            {
                UpdateUiAfterPlay(cardsToPlay);
                UiState.HasError = false;
                UiState.ErrorMessage = null;
            }
            else
            {
                var errorMessage = actionResultMessageMapper.ToString(actionResult.MessageKey);
                if (actionResult.MessageKey == ActionResultMessageKey.InvalidPlay)
                    errorMessage = errorMessage.Replace("{Card}", cardComponent.Card.ToString());

                UiState.HasError = true;
                UiState.ErrorMessage = errorMessage;
            }

            ResetSelectedCards();
            LogGameState();

            isProcessing = false;

            return actionResult.IsSuccess;
        }

        public void SelectSuit(Suit suit)
        {
            isProcessing = true;

            var actionResult = game.SelectSuit(new SelectSuitContext(CurrentTurn.PlayerToPlay, suit));

            if (actionResult.IsSuccess)
            {
                UiState.HasError = false;
                UiState.ErrorMessage = null;
            }
            else
            {
                UiState.HasError = true;
                UiState.ErrorMessage = actionResultMessageMapper.ToString(actionResult.MessageKey);
            }

            ResetSelectedCards();

            isProcessing = false;
        }

        public ActionResult Take()
        {
            isProcessing = true;

            var actionResult = game.Take(new TakeContext(CurrentTurn.PlayerToPlay));

            if (actionResult.IsSuccess)
            {
                UiState.HasError = false;
                UiState.ErrorMessage = null;
            }
            else
            {
                UiState.HasError = true;
                UiState.ErrorMessage = actionResultMessageMapper.ToString(actionResult.MessageKey);
            }

            if (actionResult.IsSuccess)
            {
                var cardComponent = UiState.CardGameObjects[game.GameState.CurrentTurn.TakenCard];
                cardComponent.OnClick = () => Play(cardComponent);
            }

            ResetSelectedCards();

            isProcessing = false;

            return actionResult;
        }

        public void SelectCard(CardComponent cardComponent)
        {
            if (game.GameState.CurrentTurn.CurrentAction != Action.Play)
                return;

            if (!game.GameState.CurrentPlayerToPlay.Hand.Cards.Contains(cardComponent.Card))
                return;

            if (selectedCards.Contains(cardComponent))
                selectedCards.Remove(cardComponent);
            else
                selectedCards.Add(cardComponent);

            cardComponent.ToggleSelected();
        }

        private void ResetSelectedCards()
        {
            foreach (var selectedCard in selectedCards)
                selectedCard.ToggleSelected();

            selectedCards.Clear();
        }

        private void UpdateUiAfterPlay(IEnumerable<CardComponent> cards)
        {
            var cardComponents = cards as CardComponent[] ?? cards.ToArray();
            
            foreach (var cardComponent in cardComponents)
                cardComponent.IsVisible = false;
            
            BringToTop(cardComponents.Last());
        }
        
        private void BringToTop(CardComponent cardComponent)
        {
            UiState.GameObjects.Remove(cardComponent);
            UiState.GameObjects.Add(cardComponent);
        }
    }
}
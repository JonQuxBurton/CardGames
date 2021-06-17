using System.Collections.Generic;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class ActionResultMessageMapper
    {
        private readonly Dictionary<ActionResultMessageKey, string> errorMessages;

        public ActionResultMessageMapper()
        {
            errorMessages = new Dictionary<ActionResultMessageKey, string>
            {
                {ActionResultMessageKey.CardIsNotInPlayersHand, "Card is not in the current Players hand"},
                {ActionResultMessageKey.InvalidPlay, "You cannot play the Cards: {Cards}"},
                {ActionResultMessageKey.NotPlayersTurn, "It is not this Player's turn"},
                {ActionResultMessageKey.InvalidTake, "You cannot Take a Card at this time"}
            };

        }

        public string ToString(ActionResultMessageKey key)
        {
            return errorMessages[key];
        }
    }
}

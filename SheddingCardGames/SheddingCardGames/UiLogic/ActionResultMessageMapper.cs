using System.Collections.Generic;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class ActionResultMessageMapper
    {
        private readonly Dictionary<CommandIsValidResultMessageKey, string> errorMessages;

        public ActionResultMessageMapper()
        {
            errorMessages = new Dictionary<CommandIsValidResultMessageKey, string>
            {
                {CommandIsValidResultMessageKey.CardIsNotInPlayersHand, "Card is not in the current Players hand"},
                {CommandIsValidResultMessageKey.InvalidPlay, "You cannot play the Cards: {Cards}"},
                {CommandIsValidResultMessageKey.NotPlayersTurn, "It is not this Player's turn"},
                {CommandIsValidResultMessageKey.InvalidTake, "You cannot Take a Card at this time"},
                {CommandIsValidResultMessageKey.GameCompleted, "The Game has been completed"}
            };

        }

        public string ToString(CommandIsValidResultMessageKey key)
        {
            return errorMessages[key];
        }
    }
}

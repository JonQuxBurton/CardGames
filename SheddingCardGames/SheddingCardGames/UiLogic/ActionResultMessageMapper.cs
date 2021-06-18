using System.Collections.Generic;
using SheddingCardGames.Domain;

namespace SheddingCardGames.UiLogic
{
    public class ActionResultMessageMapper
    {
        private readonly Dictionary<CommandExecutionResultMessageKey, string> errorMessages;

        public ActionResultMessageMapper()
        {
            errorMessages = new Dictionary<CommandExecutionResultMessageKey, string>
            {
                {CommandExecutionResultMessageKey.CardIsNotInPlayersHand, "Card is not in the current Players hand"},
                {CommandExecutionResultMessageKey.InvalidPlay, "You cannot play the Cards: {Cards}"},
                {CommandExecutionResultMessageKey.NotPlayersTurn, "It is not this Player's turn"},
                {CommandExecutionResultMessageKey.InvalidTake, "You cannot Take a Card at this time"}
            };

        }

        public string ToString(CommandExecutionResultMessageKey key)
        {
            return errorMessages[key];
        }
    }
}

using System.Text.Json.Serialization;

namespace CardGamesDomain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Rank
    {
        ACE,
        TWO,
        THREE,
        FOUR,
        FIVE,
        SIX,
        SEVEN,
        EIGHT,
        NINE,
        TEN,
        JACK,
        QUEEN,
        KING
    }
}
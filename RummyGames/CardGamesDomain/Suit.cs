using System.Text.Json.Serialization;

namespace CardGamesDomain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Suit
    {
        CLUBS,
        DIAMONDS,
        HEARTS,
        SPADES
    }
}
namespace SheddingCardGames.Domain
{
    public static class CardMoveSources
    {
        public static string StockPile = "StockPile";
        public static string DiscardPile = "DiscardPile";

        public static string PlayerHand(int number) => $"PlayerHand_{number}";
    }
}
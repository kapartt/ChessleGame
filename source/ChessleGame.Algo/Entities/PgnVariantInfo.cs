namespace ChessleGame.Algo.Entities
{
    public class PgnVariantInfo
    {
        public PgnVariantInfo(int totalCount = 0, string gameInfo = "")
        {
            TotalCount = totalCount;
            GameInfo = gameInfo;
        }

        public int TotalCount { get; set; }
        public string GameInfo { get; set; }
    }
}

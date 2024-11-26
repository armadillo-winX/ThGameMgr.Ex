namespace ThGameMgr.Ex.Score
{
    internal class SpellCardStatisticsData
    {
        /// <summary>
        /// 取得したことのあるスペルカードの数
        /// </summary>
        public int GetCardCount { get; set; }

        /// <summary>
        /// 挑戦したことのあるスペルカードの数
        /// </summary>
        public int TriedCardCount { get; set; }

        /// <summary>
        /// 取得したことのあるスペルカードの挑戦したことのあるスペルカードに対する割合(%)
        /// </summary>
        public string? GetCardCountRate { get; set; }

        /// <summary>
        /// スペルカード取得数の合計
        /// </summary>
        public int TotalGetCount { get; set; }

        /// <summary>
        /// スペルカード挑戦数の合計
        /// </summary>
        public int TotalTryCount { get; set; }

        /// <summary>
        /// 合計挑戦数に対する合計取得数の割合(%)
        /// </summary>
        public string? TotalGetCountRate { get; set; }
    }
}

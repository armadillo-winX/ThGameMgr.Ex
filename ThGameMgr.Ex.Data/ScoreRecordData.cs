namespace ThGameMgr.Ex.Data
{
    public class ScoreRecordData
    {
        /// <summary>
        /// スコア値
        /// </summary>
        public string? Score { get; set; }

        /// <summary>
        /// 自機名
        /// </summary>
        public string? Player { get; set; }

        /// <summary>
        /// 難易度
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// 名前
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// 到達面
        /// </summary>
        public string? Progress { get; set; }

        /// <summary>
        /// 日時
        /// </summary>
        public string? Date { get; set; }

        /// <summary>
        /// 処理落ち率
        /// </summary>
        public string? SlowRate { get; set; }

        /// <summary>
        /// その他のデータ
        /// </summary>
        public string? OtherData { get; set; }
    }
}

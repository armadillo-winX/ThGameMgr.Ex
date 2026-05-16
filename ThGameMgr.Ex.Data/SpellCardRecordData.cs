namespace ThGameMgr.Ex.Data
{
    public class SpellCardRecordData
    {
        /// <summary>
        /// スペルカードの番号
        /// </summary>
        public string? CardID { get; set; }

        /// <summary>
        /// スペルカード名
        /// </summary>
        public string? CardName { get; set; }

        /// <summary>
        /// 挑戦回数
        /// </summary>
        public string? TryCount { get; set; }

        /// <summary>
        /// 取得回数
        /// </summary>
        public string? GetCount { get; set; }

        /// <summary>
        /// 取得率(百分率)
        /// </summary>
        public string? Rate { get; set; }

        /// <summary>
        /// スペルカードの出現難易度
        /// </summary>
        public string? Level { get; set; }

        /// <summary>
        /// スペルカードの発動場所
        /// </summary>
        public string? Place { get; set; }

        /// <summary>
        /// スペルカードの術者
        /// </summary>
        public string? Enemy { get; set; }
    }
}

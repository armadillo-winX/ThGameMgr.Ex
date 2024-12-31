namespace ThGameMgr.Ex.Data
{
    public class ClearRecordData
    {
        /// <summary>
        /// 自機名
        /// </summary>
        public string? Player { get; set; }

        /// <summary>
        /// 難易度 Easy でのクリア達成度
        /// </summary>
        public string? Easy { get; set; }

        /// <summary>
        /// 難易度 Normal でのクリア達成度
        /// </summary>
        public string? Normal { get; set; }

        /// <summary>
        /// 難易度 Hard でのクリア達成度
        /// </summary>
        public string? Hard { get; set; }

        /// <summary>
        /// 難易度 Lunatic でのクリア達成度
        /// </summary>
        public string? Lunatic { get; set; }

        /// <summary>
        /// 難易度 Extra でのクリア達成度
        /// </summary>
        public string? Extra { get; set; }
    }
}

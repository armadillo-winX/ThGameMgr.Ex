namespace ThGameMgr.Ex.Plugin
{
    public abstract class SelectedGamePluginBase
    {
        /// <summary>
        /// プラグインの名前
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// プラグインのバージョン
        /// </summary>
        public abstract string Version { get; }

        /// <summary>
        /// プラグイン開発者名
        /// </summary>
        public abstract string Developer { get; }

        /// <summary>
        /// プラグインの説明
        /// </summary>
        public abstract string Description { get; }

        /// <summary>
        /// メニューに表示されるテキスト
        /// </summary>
        public abstract string CommandName { get; }

        /// <summary>
        /// プラグイン呼び出し時に実行される関数
        /// </summary>
        /// <param name="gamePlayLogFile"></param>
        public abstract void Main(string gameId, string gameFile);
    }
}

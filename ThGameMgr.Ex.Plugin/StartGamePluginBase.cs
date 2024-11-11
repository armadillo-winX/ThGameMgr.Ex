﻿using System.Diagnostics;

namespace ThGameMgr.Ex.Plugin
{
    public abstract class StartGamePluginBase
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
        /// <param name="gameId"></param>
        /// <param name="gameFilePath"></param>
        /// <returns></returns>
        public abstract Process Main(string gameId, string gameFilePath);
    }
}

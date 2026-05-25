namespace ThGameMgr.Ex
{
    internal class PathInfo
    {
        /// <summary>
        /// 東方管制塔 EX のアセンブリのファイルパスを取得します．
        /// </summary>
        public static string AssemblyFilePath => typeof(App).Assembly.Location;

        /// <summary>
        /// 東方管制塔 EX のアセンブリがあるディレクトリのパスを取得します．
        /// </summary>
        public static string AssemblyBaseDirectoryPath => AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// ユーザーディレクトリのパスを取得します．
        /// </summary>
        public static string UsersDirectory => $"{AssemblyBaseDirectoryPath}\\Users";

        /// <summary>
        /// UsersIndex.xml のファイルパスを取得します．
        /// </summary>
        public static string UsersIndexFile => $"{AssemblyBaseDirectoryPath}\\UsersIndex.xml";

        /// <summary>
        /// UserSelectionConfig.xml のファイルパスを取得します．
        /// </summary>
        public static string UserSelectionConfigFile => $"{AssemblyBaseDirectoryPath}\\UserSelectionConfig.xml";

        /// <summary>
        /// AppData の ShanghaiAlice フォルダのパスを取得します．
        /// </summary>
        public static string ShanghaiAliceAppData => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ShanghaiAlice";

        /// <summary>
        /// スペルカードデータファイルが格納されたディレクトリのパスを取得します．
        /// </summary>
        public static string SpellCardDataDirectory => $"{AssemblyBaseDirectoryPath}\\SpellCardData";

        /// <summary>
        /// 東方管制塔 EX プラグイン格納ディレクトリのパスを取得します．
        /// </summary>
        public static string PluginDirectory => $"{AssemblyBaseDirectoryPath}\\Plugin";
    }
}

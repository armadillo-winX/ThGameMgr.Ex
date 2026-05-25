namespace ThGameMgr.Ex
{
    internal class PathInfo
    {

        public static string AssemblyFilePath => typeof(App).Assembly.Location;

        public static string AssemblyBaseDirectoryPath => AppDomain.CurrentDomain.BaseDirectory;

        public static string UsersDirectory => $"{AssemblyBaseDirectoryPath}\\Users";

        public static string UsersIndexFile => $"{AssemblyBaseDirectoryPath}\\UsersIndex.xml";

        public static string UserSelectionConfigFile => $"{AssemblyBaseDirectoryPath}\\UserSelectionConfig.xml";

        public static string ShanghaiAliceAppData => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ShanghaiAlice";

        public static string SpellCardDataDirectory => $"{AssemblyBaseDirectoryPath}\\SpellCardData";

        public static string PluginDirectory => $"{AssemblyBaseDirectoryPath}\\Plugin";
    }
}

namespace ThGameMgr.Ex
{
    internal class PathInfo
    {

        public static string AppPath => typeof(App).Assembly.Location;

        public static string? AppLocation => Path.GetDirectoryName(AppPath);

        public static string? UsersDirectory => $"{AppLocation}\\Users";

        public static string? UsersIndexFile => $"{AppLocation}\\UsersIndex.xml";

        public static string? UserSelectionConfigFile => $"{AppLocation}\\UserSelectionConfig.xml";

        public static string ShanghaiAliceAppData => $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\ShanghaiAlice";

        public static string SpellCardDataDirectory => $"{AppLocation}\\SpellCardData";

        public static string PluginDirectory => $"{AppLocation}\\Plugin";
    }
}

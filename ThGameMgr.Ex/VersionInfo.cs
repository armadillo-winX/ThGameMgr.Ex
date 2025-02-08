using System.Runtime.InteropServices;

namespace ThGameMgr.Ex
{
    internal class VersionInfo
    {
        private static readonly string _appPath = PathInfo.AppPath;

        public static string? AppName => FileVersionInfo.GetVersionInfo(_appPath).ProductName;

        public static string CodeName => "Seiran";

        public static string? AppVersion => FileVersionInfo.GetVersionInfo(_appPath).ProductVersion;

        public static string? Developer => FileVersionInfo.GetVersionInfo(_appPath).CompanyName;

        public static string? Copyright => FileVersionInfo.GetVersionInfo(_appPath).LegalCopyright;

        public static string OSVersion => Environment.OSVersion.ToString();

        public static string DotNetVersion => RuntimeInformation.FrameworkDescription;

        public static string SystemArchitecture => RuntimeInformation.OSArchitecture.ToString();

        public static string AppArchitecture => RuntimeInformation.ProcessArchitecture.ToString();
    }
}
